using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroServicio.Reservaciones.Errors;
using MicroServicio.Reservaciones.Services;

namespace MicroServicio.Reservaciones.Messages.Producers
{
    public class RabbitMQErrorReservation : IDisposable
    {
        private readonly RabbitMQService _service;
        public RabbitMQErrorReservation(RabbitMQService service)
        {
            _service = service;
        }

        public async Task PublishErrorReservationAsync(ErrorResevation error)
        {
            await _service.PublishErrorAsync(error,"ErrorReservation");
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}