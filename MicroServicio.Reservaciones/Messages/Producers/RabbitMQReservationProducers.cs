using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroServicio.Reservaciones.DTOs;
using MicroServicio.Reservaciones.Services;

namespace MicroServicio.Reservaciones.Messages.Producers
{
    public class RabbitMQReservationProducers : IDisposable
    {

        private readonly RabbitMQService _rabbitMQ;
        public RabbitMQReservationProducers(RabbitMQService service)
        {

            _rabbitMQ = service;
        }

        public void Dispose()
        {
            _rabbitMQ.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task PublisResevation(ResponseReservation response)
        {   
            //!Creo que este ni se ocupa pero igual checa
            await _rabbitMQ.PublishAsync<ResponseReservation>(response,"viaje_registrado_queue");
        }
    
    }
}