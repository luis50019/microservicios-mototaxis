using System;
using System.Text.Json;
using System.Threading.Tasks;
using MicroServicio.Reservaciones.Config;
using MicroServicio.Reservaciones.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RabbitMQ.Client;

namespace MicroServicio.Reservaciones.Services
{
    public class ReservationService : IDisposable
    {
        private readonly IMongoCollection<Reservation> _reservationsCollection;
        private readonly IConnection _rabbitConnection;
        private readonly IModel _rabbitChannel;
        private readonly string _queueName;

        public ReservationService(
            IOptions<MongoDBSettings> mongoSettings,
            IOptions<RabbitMQSettings> rabbitSettings)
        {
            var mongoClient = new MongoClient(mongoSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoSettings.Value.Database);
            _reservationsCollection = mongoDatabase.GetCollection<Reservation>("reservationsPrivate");

            _queueName = rabbitSettings.Value.QueueName;

            var factory = new ConnectionFactory()
            {
                Uri = new Uri(rabbitSettings.Value.url),
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                RequestedHeartbeat = TimeSpan.FromSeconds(60),
                RequestedConnectionTimeout = TimeSpan.FromSeconds(30)
            };
            _rabbitConnection = factory.CreateConnection();
            _rabbitChannel = _rabbitConnection.CreateModel();

            _rabbitChannel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false);
        }

        // Registra la reserva y publica mensaje en cola
        public async Task RegisterReservationAsync(Reservation reservation)
        {
            if (reservation.Route == null)
                throw new ArgumentException("La ruta no puede ser nula.");

            if (reservation.Route.Distance < 0)
                throw new ArgumentException("La distancia no puede ser negativa.");

            const double maxDistanceMeters = 500000; // 500 km max

            if (reservation.Route.Distance > maxDistanceMeters)
                throw new ArgumentException("La distancia es excesivamente grande.");

            reservation.CreatedAt = DateTime.UtcNow;
            reservation.UpdatedAt = DateTime.UtcNow;

            await _reservationsCollection.InsertOneAsync(reservation);

            var message = new
            {
                IdReservation = reservation.Id,
                IdClient = reservation.Passage,
                IdDriver = reservation.Driver,
                IdRideFare = reservation.Rate,
                Distance = reservation.Route.Distance
            };

            string messageJson = JsonSerializer.Serialize(message);
            var body = System.Text.Encoding.UTF8.GetBytes(messageJson);

            _rabbitChannel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: null, body: body);
        }

        public void Dispose()
        {
            _rabbitChannel?.Close();
            _rabbitConnection?.Close();
        }
    }
}
