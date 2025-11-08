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
    public class RabbitMQCompletedTripConsumer
    {
        private readonly RabbitMQService _rabbitMQ;
        private readonly RabbitMQErrorReservation _rabbitMQErro;
        private readonly RabbitMQErrorReservation _rabbitMQError;
        private readonly MongoService _mongoService;

        public RabbitMQCompletedTripConsumer(RabbitMQService service, RabbitMQErrorReservation rabbitMQErro, MongoService mongoService, RabbitMQErrorReservation rabbitMQError)
        {
            _rabbitMQ = service;
            _rabbitMQErro = rabbitMQErro;
            _rabbitMQError = rabbitMQError;
            _mongoService = mongoService;
        }


        //?Metodo que recibe la respuesta de la cola de RabbitMQ
        public async Task StartConsumingCompletedTrip()
        {

            await _rabbitMQ.StartConsuming<CompletedTripDTO>("FinishTrip", async (msg) =>
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
                    var response = await _mongoService.CompletedTrip(msg);
                    if (!response)
                    {
                        throw new Exception("Error al guardar la reservacion");
                    }

                    await _rabbitMQ.PublishAsync<ResponseCompletedTripDTO>(new ResponseCompletedTripDTO
                    {
                        IdClient = msg.IdClient,
                        IdDriver = msg.IdDriver,
                        Message = "Reservacion finalizada",
                        StateUpdate = true
                    }, "FinishReservation");
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