using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Channels;
using System.Threading.Tasks;
using MicroServicio.Tarifas.Config;
using MicroServicio.Tarifas.DTOs;
using MicroServicio.Tarifas.Models;
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
        private readonly RideFaresService _service;

        public RabbitMQService(IOptions<RabbitMQSettings> settings, RideFaresService service)
        {
            var factory = new ConnectionFactory()
            {
                Uri = new Uri("amqps://vcbmhysr:BdYuwAJ4qpXfRIapENgqZlbFtGda2wF0@fly.rmq.cloudamqp.com/vcbmhysr"),

                RequestedHeartbeat = TimeSpan.FromSeconds(60),
                RequestedConnectionTimeout = TimeSpan.FromSeconds(30),
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };
            // Mejor usar async/await correctamente
            _connection = Task.Run(async () => await factory.CreateConnectionAsync()).Result;
            _channel = Task.Run(async () => await _connection.CreateChannelAsync()).Result;
            _queueName = settings.Value.QueueName;

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
            _service = service;
        }

        //!Método para poder publicar una cola en RabbitMQ
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

        //!Método para comenzar a consumir los mensajes de las colas de RabbitMQ
        public async Task StartConsuming()
        {

            // Configurar QoS antes de consumir
            await _channel.BasicQosAsync(0, 1, false);
            Console.WriteLine("consumiendo");
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var rideFareMessage = JsonSerializer.Deserialize<RideFareMessage>(message);
                    Console.WriteLine($" 📩 Mensaje: {message}");

                    // Procesar mensaje aquí
                    await ProcessMessage(rideFareMessage);

                    // Confirmar procesamiento
                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
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

        //!metodo que se encarga de procesar el mensaje recibido
        private async Task ProcessMessage(RideFareMessage message)
        {
            try
            {
                //?el mensaje lo transformacmos a un objeto
                if (message == null)
                {
                    Console.WriteLine("El mensaje es null");
                    throw new Exception("No se logro obtener la informacion");
                }

                //?buscamos la tarifa
                var response = await _service.GetRideFareAsync(message.distanceTraveled);

                if (response == null)
                {
                    Console.WriteLine("entro al null del response");
                    throw new Exception("No se encontro la tarifa");
                }
                Console.WriteLine($"info: {response.DistanceMin}");

                //?Preparamos el mensaje que sera la respuesta
                var responseMessage = new FareResponseMessage
                {
                    IdUser = message.IdUser,
                    Success = response != null,
                    Fare = response,
                    ErrorMessage = response == null ? "No se encontro la tarifa" : string.Empty

                };

                //?serializamos la respuesta
                var responseBody = JsonSerializer.Serialize(responseMessage);
                var responseBytes = Encoding.UTF8.GetBytes(responseBody);

                //?publicamos el mensajes
                await _channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: "fare_response_queue",
                    mandatory: true,
                    basicProperties: new BasicProperties { Persistent = true }, //le decimos que el mensaje deb
                    body: responseBytes
                );

                Console.WriteLine($"publicamos para el usuario {message.IdUser}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                var errorResponse = new FareResponseMessage
                {
                    IdUser = string.Empty,
                    Success = false,
                    Fare = null,
                    ErrorMessage = $"Error procesando el mensaje {ex.Message}"
                };

                var responseBody = JsonSerializer.Serialize(errorResponse);
                var responseBytes = Encoding.UTF8.GetBytes(responseBody);

                //?publicamos el mensajes
                await _channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: "fare_response_queue",
                    mandatory: true,
                    basicProperties: new BasicProperties { Persistent = true }, //le decimos que el mensaje deb
                    body: responseBytes
                );
            }

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