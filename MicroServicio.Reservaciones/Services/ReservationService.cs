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
            Console.WriteLine("====================================================");
            _consumer.StartCosumingAsync();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}