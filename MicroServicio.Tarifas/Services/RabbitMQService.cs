using System.Text;
using System.Text.Json;
using MicroServicio.Tarifas.Config;
using MicroServicio.Tarifas.DTOs;
using MicroServicio.Tarifas.Errors;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

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
            try
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
                _service = service;

                Task.Run(async () =>
                {
                    await _channel.ExchangeDeclareAsync("solicitud_viaje", ExchangeType.Direct, durable: true);

                    await _channel.QueueDeclareAsync("fare_response_queue", false, false, false, null);
                    await _channel.QueueDeclareAsync("ErrorRideFare", false, false, false, null);
                }).Wait();

                _channel.BasicQosAsync(0, 1, false).GetAwaiter().GetResult();
            }
            catch (BrokerUnreachableException ex)
            {
                Console.WriteLine($"❌ No se pudo conectar a RabbitMQ: {ex.Message}");
                throw new CustomError(
                    MessageError: "No se pudo conectar al servicio de mensajería.",
                    DetailError: ex.Message,
                    Suggest: "Verifica que RabbitMQ esté activo y la URI sea correcta.",
                    IdClient: "system",
                    CodeStatus: 500
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error inicializando RabbitMQService: {ex.Message}");
                throw new CustomError(
                    MessageError: "Error inesperado al inicializar RabbitMQService.",
                    DetailError: ex.Message,
                    Suggest: "Contacta al administrador del sistema.",
                    IdClient: "system",
                    CodeStatus: 500
                );
            }
        }


        //!Método para poder publicar una cola en RabbitMQ
        public async Task PublishAsync(string message)
        {
            try
            {
                var body = System.Text.Encoding.UTF8.GetBytes(message);
                await _channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: "solicitud_viaje", //se coloca el nombre de la cola
                    mandatory: true,
                    basicProperties: new BasicProperties { Persistent = false }, //le decimos que el mensaje debe de persistir dentro de la cola
                    body: body
                );
            }
            catch (BrokerUnreachableException ex)
            {
                Console.WriteLine($"❌ No se pudo conectar a RabbitMQ: {ex.Message}");

                //! publicar error en la cola de errores
                await PublishErrorRideFareAsync(
                    IdClient: "system",
                    MessageError: "No se pudo conectar a RabbitMQ para publicar el mensaje.",
                    DetailError: ex.Message,
                    Suggest: "Verifica que RabbitMQ esté activo y la URI sea correcta.",
                    CodeStatus: 500
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al publicar mensaje: {ex.Message}");

                await PublishErrorRideFareAsync(
                    IdClient: "system",
                    MessageError: "Error inesperado al publicar mensaje en RabbitMQ.",
                    DetailError: ex.Message,
                    Suggest: "Contacta con el administrador del sistema.",
                    CodeStatus: 500
                );
            }
        }

        //! Método para verificar el estado
        public async void CheckConnection()
        {
            try
            {
                Console.WriteLine($"Connection open: {_connection?.IsOpen ?? false}");
                Console.WriteLine($"Channel open: {_channel?.IsOpen ?? false}");

                if (_channel != null && _channel.IsOpen)
                {
                    var queueInfo = _channel.QueueDeclarePassiveAsync(_queueName);
                    Console.WriteLine($"Messages in queue '{_queueName}': {queueInfo}");
                }
                else
                {
                    throw new CustomError(
                        MessageError: "El canal de RabbitMQ no está abierto.",
                        DetailError: "El canal es null o cerrado.",
                        Suggest: "Verifica que la conexión con RabbitMQ esté activa.",
                        IdClient: "system",
                        CodeStatus: 500
                    );
                }
            }
            catch (BrokerUnreachableException ex)
            {
                await PublishErrorRideFareAsync(
                    IdClient: "system",
                    MessageError: "No se pudo conectar a RabbitMQ.",
                    DetailError: ex.Message,
                    Suggest: "Verifica que RabbitMQ esté activo y la URI sea correcta.",
                    CodeStatus: 500
                );
            }
            catch (CustomError ce)
            {
                await PublishErrorRideFareAsync(
                    IdClient: ce.IdClient,
                    MessageError: ce.MessageError,
                    DetailError: ce.DetailError,
                    Suggest: ce.Suggest,
                    CodeStatus: ce.CodeStatus
                );
            }
            catch (Exception ex)
            {
                await PublishErrorRideFareAsync(
                    IdClient: "system",
                    MessageError: "Error inesperado al verificar el estado de RabbitMQ.",
                    DetailError: ex.Message,
                    Suggest: "Contacta con el administrador del sistema.",
                    CodeStatus: 500
                );
            }
        }

        //!Método para comenzar a consumir los mensajes de las colas de RabbitMQ
        public async Task StartConsuming()
        {
            try
            {
                Console.WriteLine("Comenzando a consumir mensajes de RabbitMQ...");

                await _channel.QueueDeclareAsync(queue: "solicitud_viaje", durable: false, exclusive: false, autoDelete: false);

                await _channel.QueueBindAsync("solicitud_viaje", "solicitud_viaje", routingKey: string.Empty);

                //** Declaramos el consumer y comenzamos a consumir los mensajes enviados
                var consumer = new AsyncEventingBasicConsumer(_channel);


                consumer.ReceivedAsync += async (sender, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);

                        var rideFareMessage = JsonSerializer.Deserialize<RideFareMessage>(message);
                        if (rideFareMessage == null)
                        {
                            throw new CustomError(
                                MessageError: "Mensaje inválido",
                                DetailError: $"No se pudo deserializar el mensaje: {message}",
                                Suggest: "Verifica que los mensajes tengan el formato correcto",
                                IdClient: "system",
                                CodeStatus: 400
                            );
                        }

                        await ProcessMessage(rideFareMessage);

                        await _channel.BasicAckAsync(ea.DeliveryTag, false);
                    }
                    catch (CustomError ce)
                    {
                        Console.WriteLine($"❌ Error procesando mensaje: {ce.MessageError}");
                        await PublishErrorRideFareAsync(
                            IdClient: ce.IdClient,
                            MessageError: ce.MessageError,
                            DetailError: ce.DetailError,
                            Suggest: ce.Suggest,
                            CodeStatus: ce.CodeStatus
                        );

                        // Rechazar mensaje y no reintentar
                        await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Error inesperado procesando mensaje: {ex.Message}");
                        await PublishErrorRideFareAsync(
                            IdClient: "system",
                            MessageError: "Error inesperado procesando mensaje",
                            DetailError: ex.Message,
                            Suggest: "Contacta con el administrador del sistema",
                            CodeStatus: 500
                        );

                        await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
                    }
                };

                await _channel.BasicConsumeAsync(
                    queue: _queueName,
                    autoAck: false,
                    consumer: consumer
                );

                Console.WriteLine($"[*] Esperando mensajes en '{_queueName}'...");
            }
            catch (BrokerUnreachableException ex)
            {
                Console.WriteLine($"❌ No se pudo conectar a RabbitMQ: {ex.Message}");
                await PublishErrorRideFareAsync(
                    IdClient: "system",
                    MessageError: "No se pudo iniciar el consumo",
                    DetailError: ex.Message,
                    Suggest: "Verifica que RabbitMQ esté activo",
                    CodeStatus: 500
                );
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error crítico al iniciar el consumo: {ex.Message}");
                await PublishErrorRideFareAsync(
                    IdClient: "system",
                    MessageError: "Error crítico al iniciar el consumo",
                    DetailError: ex.Message,
                    Suggest: "Contacta con el administrador",
                    CodeStatus: 500
                );
                throw;
            }
        }


        //!metodo que se encarga de procesar el mensaje recibido
        private async Task ProcessMessage(RideFareMessage message)
        {
            try
            {
                Console.WriteLine("Procesando mensaje...");
                Console.WriteLine($"Mensaje recibido: {JsonSerializer.Serialize(message)}");

                //? El mensaje lo transformamos a un objeto
                if (message == null)
                {
                    Console.WriteLine("El mensaje es null");
                    throw new CustomError(
                        MessageError: "Mensaje inválido",
                        DetailError: "El mensaje recibido es null",
                        Suggest: "Verifica que los mensajes enviados a la cola tengan datos válidos",
                        IdClient: "system",
                        CodeStatus: 400
                    );
                }

                //? Buscamos la tarifa
                var response = await _service.GetRideFareAsync(message.distanceTraveled, message.locality);

                if (response == null)
                {
                    throw new CustomError(
                        MessageError: "Tarifa no encontrada",
                        DetailError: $"No se encontró tarifa para distancia {message.distanceTraveled} y localidad {message.locality}",
                        Suggest: "Verifica la configuración de tarifas en el sistema",
                        IdClient: message.IdUser,
                        CodeStatus: 404
                    );
                }

                Console.WriteLine($"info: {response.PricePrivate}");

                //? Preparamos el mensaje que será la respuesta
                var responseMessage = new FareResponseMessage
                {
                    IdUser = message.IdUser,
                    Success = true,
                    Fare = response,
                    ErrorMessage = string.Empty,
                    RequestId = message.RequestId
                };

                //? Serializamos la respuesta
                var responseBody = JsonSerializer.Serialize(responseMessage);
                var responseBytes = Encoding.UTF8.GetBytes(responseBody);

                //? Publicamos el mensaje
                await _channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: "fare_response_queue",
                    mandatory: true,
                    basicProperties: new BasicProperties { Persistent = true },
                    body: responseBytes
                );

                Console.WriteLine($"Publicamos para el usuario {message.IdUser}");
            }
            catch (CustomError ce)
            {
                Console.WriteLine($"❌ Error procesando mensaje: {ce.MessageError}");

                //? Publicamos error al websocket
                await PublishErrorRideFareAsync(
                    IdClient: ce.IdClient,
                    MessageError: ce.MessageError,
                    DetailError: ce.DetailError,
                    Suggest: ce.Suggest,
                    CodeStatus: ce.CodeStatus
                );

                //? Respondemos al usuario con fallo
                var errorResponse = new FareResponseMessage
                {
                    IdUser = message?.IdUser ?? "unknown",
                    Success = false,
                    Fare = null,
                    ErrorMessage = ce.MessageError,
                    RequestId = message?.RequestId
                };

                var responseBody = JsonSerializer.Serialize(errorResponse);
                var responseBytes = Encoding.UTF8.GetBytes(responseBody);

                await _channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: "fare_response_queue",
                    mandatory: true,
                    basicProperties: new BasicProperties { Persistent = true },
                    body: responseBytes
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error inesperado procesando mensaje: {ex.Message}");

                //? Publicamos error genérico
                await PublishErrorRideFareAsync(
                    IdClient: message?.IdUser ?? "system",
                    MessageError: "Error inesperado procesando mensaje",
                    DetailError: ex.Message,
                    Suggest: "Contacta al administrador del sistema",
                    CodeStatus: 500
                );

                var errorResponse = new FareResponseMessage
                {
                    IdUser = message?.IdUser ?? "unknown",
                    Success = false,
                    Fare = null,
                    ErrorMessage = "Error inesperado procesando mensaje",
                    RequestId = message?.RequestId
                };

                var responseBody = JsonSerializer.Serialize(errorResponse);
                var responseBytes = Encoding.UTF8.GetBytes(responseBody);

                await _channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: "fare_response_queue",
                    mandatory: true,
                    basicProperties: new BasicProperties { Persistent = true },
                    body: responseBytes
                );
            }
        }


        //! error publish
        public async Task PublishErrorRideFareAsync(string IdClient, string MessageError, string DetailError, string Suggest, int CodeStatus)
        {
            var errorMessage = new
            {
                Event = "Error Ride Fare",
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
                routingKey: "ErrorRideFare", //! nombre de la cola de error específica
                body: body
            );

            Console.WriteLine($"Error publicado en cola 'ErrorRideFare': {MessageError}");
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