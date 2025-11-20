using MicroServicio.Reservaciones.Messages.Consumers;

namespace MicroServicio.Reservaciones.Services
{
    public class ReservationService : IDisposable
    {
        private readonly RabbitMQReservationConsumer _consumer;

        public ReservationService(
            RabbitMQReservationConsumer RabbitMQconsumer)
        {
            _consumer = RabbitMQconsumer;
        }

        public async Task StartAsync()
        {
            //** Consumiendo el mensaje de la cola de reservaciones
            Task.Run(() =>
            {
                _consumer.StartCosumingAsync(); // ejecuta consumo de reservas
            });
            //** Consumiendo el mensaje de la cola de viaje completado
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}