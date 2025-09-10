using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MicroServicio.Conductores.Config;
using MicroServicio.Conductores.Data;
using MicroServicio.Conductores.DTOs;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MicroServicio.Conductores.Services
{
    public class RabbitMQServices : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly string _queueName;
        private readonly string _queueAccept;
        private readonly string _queueReject;
        private readonly DriverService _service;

        public RabbitMQServices(IOptions<RabbitMQSettings> settings, DriverService service)
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
            _queueAccept = settings.Value.QueueAcceptTrip;
            _queueReject = settings.Value.QueueRejectTrip;

            Task.Run(async () =>
            {
                await _channel.QueueDeclareAsync(_queueName, false, false, false, null);//!tarifa aceptada
                await _channel.QueueDeclareAsync(_queueReject, false, false, false, null);//!viaje aceptado
                await _channel.QueueDeclareAsync(_queueAccept, false, false, false, null);//!viaje rechazado
            }).Wait();

            _channel.BasicQosAsync(0, 1, false).GetAwaiter().GetResult();
            _service = service;
        }

        ///TODO: metodo que escuha el mensaje tarifa calculada
        public async Task ConsumingRideFareReady()
        {
            await _channel.BasicQosAsync(0, 1, false);
            Console.WriteLine($"Esperando mensajes en la cola {_queueName}...");

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var rideFareMessage = JsonSerializer.Deserialize<RequestRideFareReady>(message);

                    Console.WriteLine($"Mensaje recibido de la cola: ClienteID = {rideFareMessage.idUser}");

                    var driver = await _service.FoundConductorAsync();
                    if (driver.id != "")
                    {
                        Console.WriteLine($"Conductor asignado: {driver.id}");
                    }
                    else
                    {
                        Console.WriteLine("no se encontro otro conductoro");
                    }
                    //await PublishDriverFoundAsync(driver);

                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error procesando mensaje: {ex}");
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
                }
            };

            // 🔥 Aquí conectamos el consumer a la cola
            await _channel.BasicConsumeAsync(
                queue: _queueName, // la cola donde llegan los mensajes
                autoAck: false,
                consumer: consumer
            );
        }

        //TODO: metodo que escucha el mensaje de conductor asignado
        public async Task AcceptedTrip()
        {
            await _channel.BasicQosAsync(0, 1, false);
            Console.WriteLine($"Esperando mensajes en la cola {_queueAccept}...");

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var driverInfo = JsonSerializer.Deserialize<RequestAcceptTrip>(message);
                    Console.WriteLine("metodo de aceptacion");
                    Console.WriteLine($"Mensaje recibido de la cola: ClienteID = {driverInfo.idDriver}");

                    var driver = await _service.AcceptRideAsync(driverInfo.idDriver);
                    if (driver != "")
                    {
                        Console.WriteLine($"Conductor asignado: {driver}");
                    }
                    else
                    {
                        Console.WriteLine("no se encontro otro conductoro");
                    }
                    //await PublishDriverFoundAsync(driver);

                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error procesando mensaje: {ex}");
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
                }
            };

            // 🔥 Aquí conectamos el consumer a la cola
            await _channel.BasicConsumeAsync(
                queue: _queueAccept, // la cola donde llegan los mensajes
                autoAck: false,
                consumer: consumer
            );
        }

        ///! Publica un evento cuando el conductor acepta el viaje
        public async Task PublishDriverAcceptedAsync(string driverId)
        {
            var result = await _service.AcceptRideAsync(driverId);
            var message = JsonSerializer.Serialize(new
            {
                Event = "DriverAccepted",
                Data = result
            });

            var body = Encoding.UTF8.GetBytes(message);

            await _channel.BasicPublishAsync(
                exchange: "",
                routingKey: "fare_response_queue",
                body: body
            );
        }

        //! Publica un evento cuando el conductor rechaza el viaje
        public async Task PublishDriverRejectedAsync(string driverId)
        {
            var result = await _service.RejectRideAsync(driverId);
            var message = JsonSerializer.Serialize(new
            {
                Event = "DriverRejected",
                Data = result
            });

            var body = Encoding.UTF8.GetBytes(message);

            await _channel.BasicPublishAsync(
                exchange: "",
                routingKey: "fare_response_queue",
                body: body
            );
        }

        //! Encuentra un conductor disponible y publica el evento en la cola
        public async Task PublishDriverFoundAsync(DriverFound driver)
        {
            var message = JsonSerializer.Serialize(new
            {
                Event = "DriverFound",
                Data = new
                {
                    id = driver.id,
                    coordinates = driver.coordinates
                }
            });

            var body = Encoding.UTF8.GetBytes(message);
            Console.WriteLine("Enviando mensaje");

            await _channel.BasicPublishAsync(
                exchange: "",
                routingKey: "driverFound",
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
