using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroServicio.Reservaciones.DTOs;
using MicroServicio.Reservaciones.Errors;
using MicroServicio.Reservaciones.Messages.Producers;
using MicroServicio.Reservaciones.Services;

namespace MicroServicio.Reservaciones.Messages.Consumers
{
    public class RabbitMQRejectTrip
    {
        private readonly RabbitMQService _rabbitMQ;
        private readonly RabbitMQErrorReservation _rabbitMQError;
        private readonly MongoService _mongoService;
        public RabbitMQRejectTrip(RabbitMQService service, RabbitMQErrorReservation rabbitMQError, MongoService mongoService)
        {
            _rabbitMQ = service;
            _mongoService = mongoService;
            _rabbitMQError = rabbitMQError;
        }
        
        public async Task RejectTripConsumer()
        {
            await _rabbitMQ.StartConsuming<RejectTripDTO>("rejectTrip", async (msg) =>
            {
                try
                {
                    //** Validamos que el mensaje no sea nulo
                    if (msg == null)
                    {
                        throw new ErrorResevation(
                        "El mensaje no contiene informacion",
                        "Intente mas tarde ...", msg.IdDriver,
                        msg.IdClient, 400); //** Arrojamos el error si el mensaje no contiene informacion
                    }
                    //** colocamos la logica de mongoService
                    var response = await _mongoService.RejectTrip(msg);
                    await _rabbitMQ.PublishAsync<ResponseCompletedTripDTO>(new ResponseCompletedTripDTO
                    {
                        IdClient = msg.IdClient,
                        IdDriver = msg.IdDriver,
                        Message = "Viaje " + msg.General,

                    }, "viaje_cancelado");
                }
                catch (ErrorResevation ex)
                {
                    _rabbitMQError.PublishErrorReservationAsync(ex);
                }
                catch (ErrorMongoService ex)
                {
                    _rabbitMQError.PublishErrorReservationAsync(new ErrorResevation(
                        ex.Detail,
                        "Intente mas tarde ...", msg.IdDriver,
                        msg.IdClient, ex.CodeStatus));
                }
                catch (Exception ex)
                {
                    _rabbitMQError.PublishErrorReservationAsync(new ErrorResevation(
                        "Error de servidor",
                        "Intente mas tarde ...", msg.IdDriver,
                        msg.IdClient, 500));
                }

            });
    }

    }
}