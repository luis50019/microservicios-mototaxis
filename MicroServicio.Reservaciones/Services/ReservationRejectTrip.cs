using MicroServicio.Reservaciones.Messages.Consumers;

namespace MicroServicio.Reservaciones.Services
{
    public class ReservationRejectTrip : IDisposable
    {
        private readonly RabbitMQRejectTrip _rejectTripConsumer;

        public ReservationRejectTrip(
            RabbitMQRejectTrip rejectTripConsumer)
        {
            _rejectTripConsumer = rejectTripConsumer;
        }
        public async Task StartRejectTripAsync()
        {
            await _rejectTripConsumer.RejectTripConsumer();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}