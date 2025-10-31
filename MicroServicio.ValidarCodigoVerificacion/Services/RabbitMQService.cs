using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MicroServicio.ValidarCodigoVerificacion.Config;
using MicroServicio.ValidarCodigoVerificacion.DTOs;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MicroServicio.ValidarCodigoVerificacion.Services
{
    public class RabbitMQService : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly string _queueName;
        private readonly string _queueResponse;

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
            _queueName = settings.Value.QueueName;
            _queueResponse = settings.Value.QueueResponse;
        }

        public async Task PublisherAsync(ResponseValidateCode response)
        {
            try
            {
                //creamos una cola con el nombre codeValidate
               await _channel.QueueDeclareAsync(
                    queue: "codeValidate",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                var messge = JsonSerializer.Serialize<ResponseValidateCode>(response);
                var body = Encoding.UTF8.GetBytes(messge);
                Console.WriteLine("Enviado informacion");
                Console.WriteLine(messge);

                await _channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: "codeValidate",
                    mandatory: true,
                    basicProperties: new BasicProperties { Persistent = false },
                    body: body
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task ConsumeAsync(Func<string, Task> handler)
        {
            await _channel.ExchangeDeclareAsync(_queueName, ExchangeType.Fanout);

            await _channel.QueueDeclareAsync(queue: _queueName, durable: false, exclusive: false, autoDelete: false);

            // Este binding es imprescindible
            await _channel.QueueBindAsync(_queueName, _queueName, routingKey: string.Empty);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var msg = Encoding.UTF8.GetString(ea.Body.ToArray());
                await handler(msg);
                await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
            };

            await _channel.BasicConsumeAsync(queue: _queueName, autoAck: false, consumer);
        }


        public void Dispose()
        {
            _channel?.CloseAsync();
            _channel?.Dispose();
            _connection?.CloseAsync();
            _connection.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}