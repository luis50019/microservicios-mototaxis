using MicroServicio.CodigoVerificacion.Configurations;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MicroServicio.Tarifas.Services
{
    public class RabbitMQService : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly string _queueNameConsume;

        public delegate Task MessageReceivedHandler(string message);
        public event MessageReceivedHandler OnMessageReceived;

        public RabbitMQService(IOptions<RabbitMQSettings> settings)
        {
            var factory = new ConnectionFactory()
            {
                Uri = new Uri("amqps://vcbmhysr:BdYuwAJ4qpXfRIapENgqZlbFtGda2wF0@fly.rmq.cloudamqp.com/vcbmhysr"),

                RequestedHeartbeat = TimeSpan.FromSeconds(60),
                RequestedConnectionTimeout = TimeSpan.FromSeconds(30),
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };
            _connection = Task.Run(async () => await factory.CreateConnectionAsync()).Result;
            _channel = Task.Run(async () => await _connection.CreateChannelAsync()).Result;

            _queueNameConsume = settings.Value.QueueNameConsume;

            _channel.QueueDeclareAsync(queue: _queueNameConsume,
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            _channel.QueueDeclareAsync(queue: "codigo_generado",
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
        }

        public async Task StartConsumingAsync()
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (_, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());

                if (OnMessageReceived != null)
                    await OnMessageReceived.Invoke(message);

                _channel.BasicAckAsync(ea.DeliveryTag, false);
            };

            _channel.BasicConsumeAsync(queue: _queueNameConsume,
                                  autoAck: false,
                                  consumer: consumer);

            await Task.CompletedTask;
        }

        public async Task PublishAsync(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            await _channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: "codigo_generado",
                mandatory:true,
                basicProperties: new BasicProperties { Persistent = true },
                body: body
            );
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