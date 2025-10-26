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
    private readonly MicroServicio.Conductores.Interfaces.IServiceDriver _service;
        private readonly int _maxRetry;
        private readonly int _retryDelayMs;
        private readonly string QueueName = "accept_trip";

    public RabbitMQServices(IOptions<RabbitMQSettings> settings, MicroServicio.Conductores.Interfaces.IServiceDriver service)
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
            _maxRetry = settings.Value.MaxRetryAttempts;
            _retryDelayMs = settings.Value.RetryDelayMs;

            Task.Run(async () =>
            {
                await _channel.QueueDeclareAsync(_queueName, false, false, false, null);//?tarifa aceptada
                await _channel.ExchangeDeclareAsync(_queueAccept, ExchangeType.Fanout, durable: false); ;//?viaje aceptado
                await _channel.QueueDeclareAsync(_queueReject, false, false, false, null);//?viaje rechazado
                await _channel.QueueBindAsync(queue: QueueName, exchange: _queueAccept, routingKey: string.Empty);

            }).Wait();

            _channel.BasicQosAsync(0, 1, false).GetAwaiter().GetResult();
            _service = service;
        }

        ///!!metodo que escuha el mensaje tarifa calculada
        public async Task ConsumingRideFareReady()
        {
            await _channel.BasicQosAsync(0, 1, false);
            Console.WriteLine($"Esperando mensajes en la cola {_queueName}...");

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                try
                {
                    Console.WriteLine("============================== mensaje recibido=================");
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(message);
                    var rideFareMessage = JsonSerializer.Deserialize<RequestRideFareReady>(message);
                    if (rideFareMessage == null)
                    {
                        Console.WriteLine("Mensaje inválido en calculated_rate: deserialización fallida");
                        await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
                        return;
                    }

                    Console.WriteLine($"Mensaje recibido de la cola: ClienteID = {rideFareMessage.fare.idUser}");

                    //* llamamos al metodo que se encarga de buscar a un conductor
                    var driver = await _service.FoundConductorAsync();
                    if (driver.id != "")
                    {
                        Console.WriteLine($"Conductor asignado: {driver.id}");
                    }
                    else
                    {
                        Console.WriteLine("no se encontro otro conductoro");
                    }

                    Console.WriteLine("=====================================================");
                    Console.WriteLine("--- Publicando conductor encontrado ---");
                    //* publica al conductor encontrado para que despues el Hub le notitifica al dicho conductor, pero tambien le enviamos el id del cliente que realizo el viaje
                    Console.WriteLine(driver.id);
                    await PublishDriverFoundAsync(driver, rideFareMessage);

                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error procesando mensaje: {ex}");
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
                }
            };

            //! Aquí conectamos el consumer a la cola
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
                    if (driverInfo == null)
                    {
                        Console.WriteLine("Mensaje inválido en accept_trip: deserialización fallida");
                        await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
                        return;
                    }
                    Console.WriteLine("metodo de aceptacion");
                    Console.WriteLine($"Mensaje recibido de la cola: ClienteID = {driverInfo.infoDriver?.data?.id}");

                     //* llamamos al metodo que se encarga de cambiar el estdo del conducor a aceptado
                     var driver = await _service.AcceptRideAsync(driverInfo.infoDriver.data.id);
                     if (driver != "")
                     {
                         Console.WriteLine($"Conductor asignado: {driver}");
                     }
                     else
                     {
                         Console.WriteLine("no se encontro otro conductoro");
                     }
                     //TODO: agregar el meotodo que publica que el conductor acepto el viaje
                     //* recibe el id del conductor y el id del cliente al que se le debe de notificar
                     await PublishDriverAcceptedAsync(driver, driverInfo.infoDriver.data.client);

                     await _channel.BasicAckAsync(ea.DeliveryTag, false);
                 }
                 catch (Exception ex)
                 {
                     Console.WriteLine($"Error procesando mensaje: {ex}");
                     await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
                 }
             };

             // Aquí conectamos el consumer a la cola
             await _channel.BasicConsumeAsync(
                 queue: _queueAccept, // la cola donde llegan los mensajes
                 autoAck: false,
                 consumer: consumer
             );
         }

        //TODO: metodo que escucha el mensaje de conductor asignado
        public async Task RejectTrip()
        {
            await _channel.BasicQosAsync(0, 1, false);
            Console.WriteLine($"Esperando mensajes en la cola {_queueReject}...");

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var driverInfo = JsonSerializer.Deserialize<RequestRejectTrip>(message);
                    if (driverInfo == null)
                    {
                        Console.WriteLine("Mensaje inválido en reject_trip: deserialización fallida");
                        await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
                        return;
                    }
                    Console.WriteLine("metodo de rechazo");
                    Console.WriteLine($"Mensaje recibido: DriverID = {driverInfo.idDriver}, ClientID = {driverInfo.idClient}, Retry = {driverInfo.retryCount}");

                    // Marcar conductor como disponible y registrar rechazo
                    var rejectResult = await _service.RejectRideAsync(driverInfo.idDriver);
                    Console.WriteLine($"Resultado de RejectRideAsync: {rejectResult}");

                    // Incrementamos el contador de reintentos
                    driverInfo.retryCount++;

                    if (driverInfo.retryCount <= _maxRetry)
                    {
                        Console.WriteLine($"Intento {driverInfo.retryCount}/{_maxRetry}: buscando nuevo conductor...");

                        // Intentamos encontrar un nuevo conductor
                        var newDriver = await _service.FoundConductorAsync();
                        if (!string.IsNullOrEmpty(newDriver.id))
                        {
                            Console.WriteLine($"Nuevo conductor encontrado: {newDriver.id}");
                            //await PublishDriverFoundAsync(newDriver, driverInfo);
                        }
                        else
                        {
                            Console.WriteLine("No se encontró conductor en este intento; republicando mensaje para reintento...");
                            // Espera opcional antes de reintentar
                            await Task.Delay(_retryDelayMs);

                            // Re-publicar el mensaje incrementado para volver a procesar
                            var requeue = JsonSerializer.Serialize(driverInfo);
                            var requeueBody = Encoding.UTF8.GetBytes(requeue);
                            await _channel.BasicPublishAsync(exchange: string.Empty, routingKey: _queueReject, body: requeueBody);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Se agotaron los reintentos ({_maxRetry}). Publicando DriverNotFound para cliente {driverInfo.idClient}...");
                        var messageNoDriver = JsonSerializer.Serialize(new
                        {
                            Event = "DriverNotFound",
                            Client = driverInfo.idClient,
                            Attempts = driverInfo.retryCount
                        });

                        var bodyNoDriver = Encoding.UTF8.GetBytes(messageNoDriver);
                        await _channel.BasicPublishAsync(exchange: string.Empty, routingKey: "driverNotFound", body: bodyNoDriver);
                    }

                    // Publicamos el evento de que el conductor rechazó (registro)
                    await PublishDriverRejectedAsync(driverInfo.idDriver, driverInfo.idClient);

                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error procesando mensaje: {ex}");
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
                }
            };

            // Aquí conectamos el consumer a la cola
            await _channel.BasicConsumeAsync(
                queue: _queueReject, // la cola donde llegan los mensajes
                autoAck: false,
                consumer: consumer
            );
        }

        ///! Publica un evento cuando el conductor acepta el viaje
        //? este metodo recibe el id del conductor que acepto el viaje demas del id del cliente del viaje
        public async Task PublishDriverAcceptedAsync(string driverId, string idClient)
        {
            var result = await _service.AcceptRideAsync(driverId);
            var message = JsonSerializer.Serialize(new
            {
                Event = "DriverAccepted",
                Data = result,
                client = idClient
            });

            var body = Encoding.UTF8.GetBytes(message);

            await _channel.BasicPublishAsync(
                exchange: "",
                routingKey: "tripAccept",
                body: body
            );
        }

        //! Publica un evento cuando el conductor rechaza el viaje
        //? este metodo recibe el id del conductor que rechazo el viaje demas del id del cliente del viaje
        public async Task PublishDriverRejectedAsync(string driverId, string idClient)
        {
            var result = await _service.RejectRideAsync(driverId);
            var message = JsonSerializer.Serialize(new
            {
                Event = "DriverRejected",
                Data = result,
                Client = idClient
            });

            var body = Encoding.UTF8.GetBytes(message);

            await _channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: "tripReject",
                body: body
            );
        }

        //! Encuentra un conductor disponible y publica el evento en la cola
        //? este metodo recibe el id del conductor que fue asignado el viaje demas del id del cliente del viaje
        public async Task PublishDriverFoundAsync(DriverFound driver,RequestRideFareReady infoRideFare)
        {
            //* El mensaje contiene el id del conducto y sus coordenadas
            Console.WriteLine("Publicando conductor encontrado...{driverFound}");
            Console.WriteLine($"{driver}");
            var message = JsonSerializer.Serialize(new
            {
                Event = "DriverFound",
                Data = new
                {
                    id = driver.id,
                    locationStart = infoRideFare.locationStart,
                    locationEnd = infoRideFare.locationEnd,
                    priceTraveled = infoRideFare.priceTraveled,
                    client = infoRideFare.fare.idUser,
                    infoPassager = infoRideFare.infoPassenger,
                    coordinates = driver.coordinates,
                    rideFare = infoRideFare.fare,
                    typeSerice = infoRideFare.typeService
                }
            });

            var body = Encoding.UTF8.GetBytes(message);
            Console.WriteLine("Enviando mensaje");
            Console.WriteLine(message);

            await _channel.BasicPublishAsync(
                exchange: string.Empty,
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
