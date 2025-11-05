using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MicroServicio.Reservaciones.Config;
using MicroServicio.Reservaciones.DTOs;
using MicroServicio.Reservaciones.Errors;
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
        private readonly string _queueNameResponse = "viaje_registrado_queue";

        public RabbitMQService(IOptions<RabbitMQSettings> settings)
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
        }

        //? Metodo para eviar la informacion correcta
        //? Informacion a enviar
        public async Task PublishAsync<T>(T responseReservation, string queueName)
        {
            try
            {
                //** Declaramos la cola 
                await _channel.QueueDeclareAsync(
                    queue: queueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                //** Serealizmos la informacion
                var response = JsonSerializer.Serialize<T>(responseReservation);
                var body = Encoding.UTF8.GetBytes(response);

                //** enviamos la información
                //? lo enviamos al exchange "" con el llave de la ruta _queueNameResponse
                await _channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: queueName,
                    mandatory: true,
                    basicProperties: new BasicProperties { Persistent = true },
                    body: body);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al publicar el mensaje de respuesta");
            }
        }

        //? Método para la publicacion de errores
        //? Parametros: Error a enviar, Nombre de la cola del error
        public async Task PublishErrorAsync(ErrorResevation ErrorResponse, string queueNameError)
        {
            try
            {
                //** Declaramos la cola 
                await _channel.QueueDeclareAsync(
                    queue: queueNameError,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                //** Serealizmos la informacion
                var response = JsonSerializer.Serialize<ErrorResevation>(ErrorResponse);
                var body = Encoding.UTF8.GetBytes(response);

                //** enviamos la información
                //? lo enviamos al exchange "" con el llave de la ruta _queueNameResponse
                await _channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: queueNameError,
                    mandatory: true,
                    basicProperties: new BasicProperties { Persistent = true },
                    body: body);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al publicar el mensaje de error");
            }
        }


        //? Método para consumir el mensaje publicado
        //? Parametros: Handler para controlar y manejar el mensaje recibido
        public async Task StartConsuming<T>(string queueName,Func<T, Task> handler)
        {

            //** Declaramos el queue al que debemos escuchar
            await _channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false);

            await _channel.QueueBindAsync(queueName, queueName, routingKey: string.Empty);

            //** Declaramos el consumer y comenzamos a consumir los mensajes enviados
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (sender, ea) =>
            {
                try
                {
                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var request = JsonSerializer.Deserialize<T>(message);
                    //** Utilizamos el handler y le paso el mensaje recibido
                    await handler(request);
                    //** Confirmamos que el mensaje fue recibido
                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
                    Console.WriteLine($"Error en procesamiento: {ex.Message}");
                }
            };

            await _channel.BasicConsumeAsync(queueName, autoAck: false, consumer);
            Console.WriteLine($"[*] Esperando mensajes en '{queueName}'...");
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