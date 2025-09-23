using System;
using System.Text.Json;
using System.Threading.Tasks;
using MicroServicio.Reservaciones.Config;
using MicroServicio.Reservaciones.Data;
using MicroServicio.Reservaciones.models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RabbitMQ.Client;

namespace MicroServicio.Reservaciones.Services
{
    public class ReservationService : IDisposable
    {
        private readonly IMongoCollection<Reservation> _reservationsCollection;
        private readonly IConnection _connection;
        private readonly IChannel _channel;
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
            _connection = Task.Run(async () => await factory.CreateConnectionAsync()).Result;
            _channel = Task.Run(async () => await _connection.CreateChannelAsync()).Result;

            Task.Run(async () =>
            {
                await _channel.QueueDeclareAsync(
                    queue: _queueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );
                await _channel.QueueDeclareAsync(
                    queue: "fare_response_queue",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );
            }).Wait();

            _channel.BasicQosAsync(0, 1, false).GetAwaiter().GetResult();
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

            await _channel.BasicPublishAsync(
                exchange: "",
                routingKey: _queueName,
                mandatory: true,
                basicProperties: new BasicProperties { Persistent = true },
                body: body);
        }

        public void Dispose()
        {
            _channel?.CloseAsync();
            _connection.CloseAsync();
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}