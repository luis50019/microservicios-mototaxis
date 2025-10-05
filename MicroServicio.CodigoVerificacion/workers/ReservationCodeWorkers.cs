using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MicroServicio.CodigoVerificacion.models;
using MicroServicio.CodigoVerificacion.Configurations;
using MicroServicio.CodigoVerificacion.DTOs;

public class ReservationWorker : BackgroundService
{
    private readonly IMongoService _mongoService;
    private readonly RabbitMQService _rabbitService;
    private readonly string _queuePublishName;

    public ReservationWorker(IMongoService mongoService, RabbitMQService rabbitService, IOptions<RabbitMQSettings> settings)
    {
        _mongoService = mongoService;
        _rabbitService = rabbitService;
        _queuePublishName = settings.Value.QueueNamePublish;

        // Suscribirse al evento de mensajes
        _rabbitService.OnMessageReceived += HandleMessageAsync;
    }

    private async Task HandleMessageAsync(RequestCode mensaje)
    {
        try
        {
            Console.WriteLine("🔔 Manejo de mensaje iniciado.");
        // Deserializar mensaje recibido

        // Validar viaje
        if (await _mongoService.ExisteViaje(mensaje.idReservations))
            return;

        // Generar codigo de verificación
        string codigo = CodigoVerificacion.GenerarCodigo();
        Console.WriteLine($"✅ Código generado: {codigo}");
        // Guardar en mongo
        var InfoDriver = await _mongoService.GuardarCodigoVerificacion(mensaje.idReservations, codigo,mensaje.idDriver);

        // Publicar mensaje y codigo
        var codigoMsg = new CodigoGeneradoMessage
        {
            Code = codigo,
            IdViaje = mensaje.idReservations,
            IdClient = mensaje.idClient,
            DataDriver = InfoDriver
        };
        
        await _rabbitService.PublishAsync(codigoMsg);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al manejar el mensaje: {ex.Message}");
            throw;
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("ReservationWorker iniciado.");
        // Iniciar consumo
        await _rabbitService.StartConsumingAsync();

        // mantener worker activo
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}