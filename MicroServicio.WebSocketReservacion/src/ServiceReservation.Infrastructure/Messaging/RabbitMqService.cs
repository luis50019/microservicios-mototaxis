using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public async Task PublicAsync(string queue, string message)
        {

            await _channel.QueueDeclareAsync(queue, durable: false, exclusive: false, autoDelete: false);
            var body = Encoding.UTF8.GetBytes(message);

            Console.WriteLine("ahora estoy aqui");

            await _channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: queue, //se coloca el nombre de la cola
                mandatory: true,
                basicProperties: new BasicProperties { Persistent = true },
                body: body
            );
        }

        //?Metodo para poder consumir una cola
        public async Task ConsumeAsync(string queue, Func<string, Task> handler)
        {
            Console.WriteLine("se esta enviando el mensaje");
            await _channel.QueueDeclareAsync(queue, durable: false, exclusive: false, autoDelete: false);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            //?metodo encagado de consumir
            consumer.ReceivedAsync += async (model, ea) =>
            {
                //!obtenemos el mensaje
                var msg = Encoding.UTF8.GetString(ea.Body.ToArray());
                //!lo mandamos ala funcion para poder procesar el mensaje
                await handler(msg);
                await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
            };

            await _channel.BasicConsumeAsync(queue, autoAck: false, consumer: consumer);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}