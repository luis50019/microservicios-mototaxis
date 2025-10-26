using System.Text;
using System.Text.Json;
using MicroServicio.CodigoVerificacion.Configurations;
using MicroServicio.CodigoVerificacion.DTOs;
using MicroServicio.CodigoVerificacion.models;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class RabbitMQService : IDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly string _queueNameConsume;

    public delegate Task MessageReceivedHandler(RequestCode message);
    public event MessageReceivedHandler OnMessageReceived;

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

        _queueNameConsume = settings.Value.QueueNameConsume;

        // Bloqueante en el ctor
        _channel.QueueDeclareAsync(_queueNameConsume, durable: false, exclusive: false, autoDelete: false, arguments: null)
                .GetAwaiter().GetResult();

        _channel.QueueDeclareAsync("codigo_generado", durable: false, exclusive: false, autoDelete: false, arguments: null)
                .GetAwaiter().GetResult();
    }

    public async Task StartConsumingAsync()
    {
        Console.WriteLine("🐇 Iniciando consumo de mensajes...");
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                var data = JsonSerializer.Deserialize<RequestCode>(message);

                Console.WriteLine($"📩 Mensaje recibido: {data?.idReservations.ToString()}");

                if (OnMessageReceived != null)
                    await OnMessageReceived.Invoke(data);

                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error procesando mensaje: {ex}");
                await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: _queueNameConsume,
            autoAck: false,
            consumer: consumer
        );

        Console.WriteLine($"✅ Consumidor escuchando en '{_queueNameConsume}'...");
    }

    public async Task PublishAsync(CodigoGeneradoMessage message)
    {
        Console.WriteLine($"📤 Publicando mensaje: {message}");
        string json = JsonSerializer.Serialize<CodigoGeneradoMessage>(message);
        var body = Encoding.UTF8.GetBytes(json);
        Console.WriteLine("Mensaje publicado.");

        await _channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: "codigo_generado",
            mandatory: true,
            basicProperties: new BasicProperties { Persistent = true },
            body: body
        );
    }

    public void Dispose()
    {
        _channel?.CloseAsync().GetAwaiter().GetResult();
        _connection?.CloseAsync().GetAwaiter().GetResult();
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
