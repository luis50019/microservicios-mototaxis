using MicroServicio.Reservaciones.Messages.Consumers;

namespace MicroServicio.Reservaciones.Services
{
    public class ReservationService : IDisposable
    {
        private readonly RabbitMQReservationConsumer _consumer;
        private readonly RabbitMQCompletedTripConsumer _consumerCompletedTripConsumer;

        public ReservationService(
            RabbitMQReservationConsumer RabbitMQconsumer,
            RabbitMQCompletedTripConsumer RabbitMQCompletedTripConsumer)
        {
            _consumer = RabbitMQconsumer;
            _consumerCompletedTripConsumer = RabbitMQCompletedTripConsumer;
        }

        public async Task StartAsync()
        {
            //** Consumiendo el mensaje de la cola de reservaciones
            _consumer.StartCosumingAsync();
            //** Consumiendo el mensaje de la cola de viaje completado
            _consumerCompletedTripConsumer.StartConsumingCompletedTrip();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}