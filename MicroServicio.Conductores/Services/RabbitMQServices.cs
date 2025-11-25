using System;
using System.Net.WebSockets;
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
            try
            {
                Console.WriteLine("🔄 Iniciando conexión con RabbitMQ...");

                var factory = new ConnectionFactory()
                {
                    Uri = new Uri(settings.Value.url ??
                        "amqps://vcbmhysr:BdYuwAJ4qpXfRIapENgqZlbFtGda2wF0@fly.rmq.cloudamqp.com/vcbmhysr"),
                    RequestedHeartbeat = TimeSpan.FromSeconds(60),
                    RequestedConnectionTimeout = TimeSpan.FromSeconds(30),
                    AutomaticRecoveryEnabled = true,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
                };

                //! Intentando conexión
                _connection = Task.Run(async () => await factory.CreateConnectionAsync()).Result;
                _channel = Task.Run(async () => await _connection.CreateChannelAsync()).Result;

                //! colas
                _queueName = settings.Value.QueueName;
                _queueAccept = settings.Value.QueueAcceptTrip;
                _queueReject = settings.Value.QueueRejectTrip;
                _maxRetry = settings.Value.MaxRetryAttempts;
                _retryDelayMs = settings.Value.RetryDelayMs;
                _service = service;

                Task.Run(async () =>
                {
                    await _channel.QueueDeclareAsync(_queueName, false, false, false, null);
                    await _channel.ExchangeDeclareAsync(_queueAccept, ExchangeType.Fanout, durable: false);
                    await _channel.QueueDeclareAsync(_queueReject, false, false, false, null);
                    await _channel.QueueBindAsync(queue: _queueName, exchange: _queueAccept, routingKey: string.Empty);
                }).Wait();

                _channel.BasicQosAsync(0, 1, false).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("No se pudo inicializar la conexión RabbitMQ.", ex);
            }
        }

        ///!!metodo que escuha el mensaje tarifa calculada
        public async Task ConsumingRideFareReady()
        {
            Console.WriteLine($"Esperando mensajes en la cola calculated_ratd e...");
            await _channel.QueueDeclareAsync(queue: "calculated_rate", durable: false, exclusive: false, autoDelete: false);

            await _channel.QueueBindAsync("calculated_rate", "calculated_rate", routingKey: string.Empty);

                //** Declaramos el consumer y comenzamos a consumir los mensajes enviados
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

                    Console.WriteLine("=====================================================");
                    Console.WriteLine("--- Publicando conductor encontrado ---");
                    //* publica al conductor encontrado para que despues el Hub le notitifica al dicho conductor, pero tambien le enviamos el id del cliente que realizo el viaje
                    await PublishDriverFoundAsync(driver, rideFareMessage);

                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error procesando mensaje: {ex}");
                    string idClient = "";
                    try
                    {
                        var body = ea.Body.ToArray();
                        var msgText = Encoding.UTF8.GetString(body);
                        var parsed = JsonSerializer.Deserialize<RequestRideFareReady>(msgText);
                        idClient = parsed?.fare?.idUser ?? "desconocido";
                    }
                    catch { idClient = "desconocido"; }

                    //! publicar error en el websocket
                    await PublishErrorDriverAsync(
                        IdClient: idClient,
                        MessageError: "Error procesando solicitud de tarifa",
                        DetailError: ex.Message,
                        Suggest: "Inténtalo nuevamente más tarde",
                        CodeStatus: 500
                    );

                    //! rechazar mensaje para evitar intentos fallidos
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
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var driverInfo = JsonSerializer.Deserialize<RequestAcceptTrip>(message);
                try
                {
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
                    Console.WriteLine($"Error procesando mensaje: {ex.Message}");

                    //! enviar error al websocket
                    string idClient = "unknown";

                    if (driverInfo?.infoDriver?.data?.client != null)
                    {
                        idClient = driverInfo.infoDriver.data.client;
                    }

                    await PublishErrorDriverAsync(
                        IdClient: idClient,
                        MessageError: "Error procesando el mensaje en AcceptedTrip",
                        DetailError: ex.ToString(),
                        Suggest: "Inténtalo más tarde o revisa la información enviada",
                        CodeStatus: 500
                    );
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
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var driverInfo = JsonSerializer.Deserialize<RequestRejectTrip>(message);
                try
                {

                    //! agregando envio de mensaje al websocket
                    if (driverInfo == null)
                    {
                        Console.WriteLine("Mensaje inválido en reject_trip: deserialización fallida");

                        await PublishErrorDriverAsync(
                            IdClient: "unknown",
                            MessageError: "Deserialización fallida en RejectTrip",
                            DetailError: "El mensaje recibido no pudo convertirse a RequestRejectTrip",
                            Suggest: "Revisa el formato del mensaje enviado",
                            CodeStatus: 400
                        );

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
                    Console.WriteLine($"Error procesando mensaje en RejectTrip: {ex.Message}");

                    //! publicar en la cola ErrorDriver
                    await PublishErrorDriverAsync(
                        IdClient: driverInfo?.idClient ?? "unknown",
                        MessageError: "Error procesando mensaje en RejectTrip",
                        DetailError: ex.ToString(),
                        Suggest: "Inténtalo más tarde o revisa la información enviada",
                        CodeStatus: 500
                    );

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
            try
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
            catch (Exception ex)
            {

                Console.WriteLine($"❌ Error publicando DriverAccepted: {ex.Message}");

                //! Publicar el error en la cola ErrorDriver
                await PublishErrorDriverAsync(
                    IdClient: idClient,
                    MessageError: "Error publicando DriverAccepted",
                    DetailError: ex.ToString(),
                    Suggest: "Inténtalo más tarde",
                    CodeStatus: 500
                );

            }
        }

        //! Publica un evento cuando el conductor rechaza el viaje
        //? este metodo recibe el id del conductor que rechazo el viaje demas del id del cliente del viaje
        public async Task PublishDriverRejectedAsync(string driverId, string idClient)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error publicando DriverRejected: {ex.Message}");

                //! Publicamos el error al WebSocket en cola ErrorDriver
                await PublishErrorDriverAsync(
                    IdClient: idClient,
                    MessageError: "Error publicando DriverRejected",
                    DetailError: ex.ToString(),
                    Suggest: "Inténtalo más tarde",
                    CodeStatus: 500
                );
            }

        }

        //! Encuentra un conductor disponible y publica el evento en la cola
        //? este metodo recibe el id del conductor que fue asignado el viaje demas del id del cliente del viaje
        public async Task PublishDriverFoundAsync(DriverFound driver, RequestRideFareReady infoRideFare)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error publicando DriverFound: {ex.Message}");

                //! Publicar el error en la cola ErrorDriver
                await PublishErrorDriverAsync(
                    IdClient: infoRideFare?.fare?.idUser ?? "unknown",
                    MessageError: "Error publicando DriverFound",
                    DetailError: ex.ToString(),
                    Suggest: "Inténtalo más tarde",
                    CodeStatus: 500
                );
            }
        }
        //! error publish
        public async Task PublishErrorDriverAsync(string IdClient, string MessageError, string DetailError, string Suggest, int CodeStatus)
        {
            var errorMessage = new
            {
                Event = "Error Driver",
                Data = new
                {
                    MessageError,
                    DetailError,
                    Suggest,
                    IdClient,
                    CodeStatus
                }
            };

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(errorMessage));
            await _channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: "ErrorDriver", //! nombre de la cola de error
                body: body
            );
            Console.WriteLine($"❗ Error publicado en cola 'ErrorDriver': {MessageError}");
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
