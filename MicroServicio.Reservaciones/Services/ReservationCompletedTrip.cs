using MicroServicio.Reservaciones.Messages.Consumers;

namespace MicroServicio.Reservaciones.Services
{
    public class ReservationCompletedTrip : IDisposable
    {
        private readonly RabbitMQCompletedTripConsumer _consumerCompletedTripConsumer;

        public ReservationCompletedTrip(
            RabbitMQCompletedTripConsumer RabbitMQCompletedTripConsumer)
        {
            _consumerCompletedTripConsumer = RabbitMQCompletedTripConsumer;
        }
        public async Task StartCompletedTripAsync()
        {
            await _consumerCompletedTripConsumer.StartConsumingCompletedTrip();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}