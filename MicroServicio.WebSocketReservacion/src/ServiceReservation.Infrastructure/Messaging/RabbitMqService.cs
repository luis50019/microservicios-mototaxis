using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ServiceReservation.Infrastructure.Configurations;

namespace ServiceReservation.Infrastructure.Messaging
{
    public class RabbitMqService : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;

        public RabbitMqService(IOptions<RabbitMqSettings> options)
        {
            var fatory = new ConnectionFactory()
            {
                Uri = new Uri(options.Value.url)
            };
            _connection = fatory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        }

        //?Metodo para realizar publicaciones
        //!recibe el nombre de la cola y el mensaje a enviar
        public async Task PublicAsync(string exchange, string message)
        {

            await _channel.ExchangeDeclareAsync(exchange, ExchangeType.Fanout, durable: false);
            var body = Encoding.UTF8.GetBytes(message);

            Console.WriteLine("ahora estoy aqui enviando mensaje " + exchange);
            Console.WriteLine("data: " + message);

            await _channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: exchange,
                mandatory: true,
                basicProperties: new BasicProperties { Persistent = false },
                body: body
            );
        }
        public async Task PublicQueAsync(string exchange, string message)
        {

            await _channel.ExchangeDeclareAsync(exchange, ExchangeType.Fanout, durable: false);
            var body = Encoding.UTF8.GetBytes(message);

            Console.WriteLine("ahora estoy aqui enviando mensaje " + exchange);
            Console.WriteLine("data: " + message);

            await _channel.BasicPublishAsync(
                exchange: exchange,
                routingKey: string.Empty,
                mandatory: false,
                basicProperties: new BasicProperties { Persistent = false },
                body: body
            );
        }
        public async Task PulblicExchangeAsync(string exchange, string message)
        {

            await _channel.ExchangeDeclareAsync(exchange, ExchangeType.Fanout, durable: false);
            var body = Encoding.UTF8.GetBytes(message);

            Console.WriteLine("ahora estoy aqui enviando mensaje " + exchange);

            await _channel.BasicPublishAsync(
                exchange: exchange,
                routingKey: string.Empty, //se coloca el nombre de la cola
                mandatory: true,
                basicProperties: new BasicProperties { Persistent = true },
                body: body
            );
        }

        //?Metodo para poder consumir una cola
        public async Task ConsumeAsync(string queue, Func<IChannel, BasicDeliverEventArgs, Task> handler)
        {
            await _channel.QueueDeclareAsync(queue, durable: false, exclusive: false, autoDelete: false);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                await handler(_channel, ea); // <-- aquí sí hacemos await real
            };

            await _channel.BasicConsumeAsync(queue, autoAck: false, consumer: consumer);
        }


        /*public async Task ConsumeAsync(string queue, Func<string, Task> handler)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (sender, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                await handler(message);
                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            };
            await _channel.BasicConsumeAsync(queue, autoAck: false, consumer: consumer);
        }*/


        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}