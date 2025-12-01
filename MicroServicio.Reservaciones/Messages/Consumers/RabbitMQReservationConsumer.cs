using MicroServicio.Reservaciones.DTOs;
using MicroServicio.Reservaciones.Errors;
using MicroServicio.Reservaciones.Messages.Producers;
using MicroServicio.Reservaciones.Services;

namespace MicroServicio.Reservaciones.Messages.Consumers
{
    public class RabbitMQReservationConsumer : IDisposable
    {
        private readonly RabbitMQService _rabbitMQ;
        private readonly RabbitMQErrorReservation _rabbitMQError;
        private readonly MongoService _mongoService;
        public RabbitMQReservationConsumer(RabbitMQService service, RabbitMQErrorReservation rabbitMQError, MongoService mongoService)
        {
            _rabbitMQ = service;
            _mongoService = mongoService;
            _rabbitMQError = rabbitMQError;
        }


        public async Task StartCosumingAsync()
        {
            await _rabbitMQ.StartConsuming<RequestReservations>("accept_trip",async (msg) =>
            {
                
                try
                {
                    //** Validamos que el mensaje no sea nulo
                    if (msg == null)
                    {
                        throw new ErrorResevation(
                        "El mensaje no contiene informacion",
                        "Intente mas tarde ...", msg.infoDriver.data.id,
                        msg.infoDriver.data.rideFare.idUser, 400); //** Arrojamos el error si el mensaje no contiene informacion
                    }
                    //** colocamos la logica de mongoService
                    var response = await _mongoService.Insert(msg);
                    await _rabbitMQ.PublishAsync<ResponseReservation>(response,"viaje_registrado_queue");
                }
                catch (ErrorResevation ex)
                {
                    Console.WriteLine(ex);
                    _rabbitMQError.PublishErrorReservationAsync(ex);
                }
                catch (ErrorMongoService ex)
                {
                    Console.WriteLine(ex);
                    _rabbitMQError.PublishErrorReservationAsync(new ErrorResevation(
                        ex.Detail,
                        "Intente mas tarde ...", msg.infoDriver.data.id,
                        msg.infoDriver.data.rideFare.idUser, ex.CodeStatus));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    _rabbitMQError.PublishErrorReservationAsync(new ErrorResevation(
                        "Error de servidor",
                        "Intente mas tarde ...", msg.infoDriver.data.id,
                        msg.infoDriver.data.rideFare.idUser,500));
                }

            });
        }
        public void Dispose()
        {
            _rabbitMQ.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}