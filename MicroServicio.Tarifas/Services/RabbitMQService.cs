using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using MicroServicio.Tarifas.Config;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MicroServicio.Tarifas.Services
{
    public class RabbitMQService : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly string _queueName;

        public RabbitMQService(IOptions<RabbitMQSettings> settings)
        {
            var factory = new ConnectionFactory()
            {
                Uri = new Uri("amqps://dmnmlqjm:N-6RnHYVqNy0erP1BVlNzDXxFgDEG205@fly.rmq.cloudamqp.com/dmnmlqjm"),

                RequestedHeartbeat = TimeSpan.FromSeconds(60),
                RequestedConnectionTimeout = TimeSpan.FromSeconds(30),
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };
            // Mejor usar async/await correctamente
            _connection = Task.Run(async () => await factory.CreateConnectionAsync()).Result;
            _channel = Task.Run(async () => await _connection.CreateChannelAsync()).Result;
            _queueName = settings.Value.QueueName;

            Task.Run(async () => await _channel.QueueDeclareAsync(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            )).Wait();

            _channel.BasicQosAsync(0, 1, false).GetAwaiter().GetResult();
            Console.WriteLine($" ✅ Conectado a RabbitMQ. Cola: {_queueName}");
            Console.WriteLine($" 🔗 Heartbeat: {factory.RequestedHeartbeat}");
            Console.WriteLine($" ⏱️ Timeout: {factory.RequestedConnectionTimeout}");
        }

        public async Task PublishAsync(string message)
        {
            var body = System.Text.Encoding.UTF8.GetBytes(message);
            await _channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: "solicitud_viaje", //se coloca el nombre de la cola
                mandatory: true,
                basicProperties: new BasicProperties { Persistent = true }, //le decimos que el mensaje debe de persistir dentro de la cola
                body: body
            );
        }
        // Método para verificar el estado
        public void CheckConnection()
        {
            Console.WriteLine($"Connection open: {_connection?.IsOpen ?? false}");
            Console.WriteLine($"Channel open: {_channel?.IsOpen ?? false}");

            if (_channel != null && _channel.IsOpen)
            {
                var queueInfo = _channel.QueueDeclarePassiveAsync(_queueName);
                Console.WriteLine($"Messages in queue '{_queueName}': {queueInfo}");
            }
        }
        public async Task StartConsuming()
        {
            Console.WriteLine(" 🟢 Iniciando consumo de la cola...");

            // Configurar QoS antes de consumir
            await _channel.BasicQosAsync(0, 1, false);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                try
                {
                    Console.WriteLine(" ✅ Mensaje recibido");
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    Console.WriteLine($" 📩 Mensaje: {message}");

                    // Procesar mensaje aquí
                    await ProcessMessage(message);

                    // Confirmar procesamiento
                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" ❌ Error: {ex.Message}");
                    // Rechazar mensaje y no reintentar
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
                }
            };

            // Iniciar consumo
            await _channel.BasicConsumeAsync(
                queue: _queueName,
                autoAck: false,  // Importante: autoAck = false
                consumer: consumer
            );

            Console.WriteLine($" [*] Esperando mensajes en '{_queueName}'...");
        }

        private async Task ProcessMessage(string message)
        {
            // Tu lógica de procesamiento aquí
            Console.WriteLine("mensaje: " + message);
            await Task.Delay(100); // Simular procesamiento
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