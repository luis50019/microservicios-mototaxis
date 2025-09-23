using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MicroServicio.Tarifas.Services;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MicroServicio.CodigoVerificacion.models;
using MicroServicio.CodigoVerificacion.Configurations;

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

    private async Task HandleMessageAsync(string mensaje)
    {
        // Deserializar mensaje recibido
        var viaje = JsonSerializer.Deserialize<Reservation>(mensaje);
        if (viaje == null) return;

        // Validar viaje
        if (!await _mongoService.ExisteViaje(viaje.Id))
            return;

        // Generar codigo de verificación
        string codigo = CodigoVerificacion.GenerarCodigo();

        // Guardar en mongo
        await _mongoService.GuardarCodigoVerificacion(viaje.Id, codigo);

        // Publicar mensaje y codigo
        var codigoMsg = new CodigoGeneradoMessage
        {
            Code = codigo,
            IdViaje = viaje.Id
        };
        string json = JsonSerializer.Serialize(codigoMsg);
        await _rabbitService.PublishAsync(json);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Iniciar consumo
        await _rabbitService.StartConsumingAsync();

        // mantener worker activo
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}