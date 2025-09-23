using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MicroServicio.Reservaciones.Config;
using MicroServicio.Reservaciones.DTOs;
using MicroServicio.Reservaciones.models;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MicroServicio.Reservaciones.Services
{
    public class RabbitMQService : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly string _queueName;
        private readonly ReservationService _reservationService;

        public RabbitMQService(IOptions<RabbitMQSettings> settings, ReservationService reservationService)
        {
            var factory = new ConnectionFactory()
            {
                Uri = new Uri(settings.Value.url),
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                RequestedHeartbeat = TimeSpan.FromSeconds(60),
                RequestedConnectionTimeout = TimeSpan.FromSeconds(30)
            };

            _connection = Task.Run(async () => await factory.CreateConnectionAsync()).Result;
            _channel = Task.Run(async () => await _connection.CreateChannelAsync()).Result;
            _queueName = settings.Value.QueueName;

            Task.Run(async () =>
            {
                await _channel.QueueDeclareAsync(_queueName, durable: false, exclusive: false, autoDelete: false);
                await _channel.QueueDeclareAsync("viaje_registrado_queue", durable: false, exclusive: false, autoDelete: false);
            }).Wait();

            _channel.BasicQosAsync(0, 1, false).GetAwaiter().GetResult();
            _reservationService = reservationService;
        }

        public async Task PublishAsync(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            await _channel.BasicPublishAsync(
                exchange: "",
                routingKey: "viaje_registrado_queue",
                mandatory: true,
                basicProperties: new BasicProperties { Persistent = true },
                body: body);
        }

        public async Task StartConsuming()
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var reservationRequest = JsonSerializer.Deserialize<ReservationMessage>(message);

                    if (reservationRequest == null)
                        throw new Exception("Mensaje de reserva inválido");

                    var reservation = new Reservation
                    {
                        Passage = reservationRequest.IdClient,
                        Driver = reservationRequest.IdDriver,
                        Rate = reservationRequest.IdRideFare,
                        Route = new Route { Distance = reservationRequest.Distance },
                        State = new State { General = "Reservado" }
                    };

                    await _reservationService.RegisterReservationAsync(reservation);
                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
                    Console.WriteLine($"Error en procesamiento: {ex.Message}");
                }
            };

            await _channel.BasicConsumeAsync(_queueName, autoAck: false, consumer);
            Console.WriteLine($"[*] Esperando mensajes en '{_queueName}'...");
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