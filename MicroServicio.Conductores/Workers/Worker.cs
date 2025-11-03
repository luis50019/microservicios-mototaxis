using System;
using System.Threading;
using System.Threading.Tasks;
using MicroServicio.Conductores.Interfaces;
using MicroServicio.Conductores.Services;
using Microsoft.Extensions.Hosting;

namespace MicroServicio.Conductores.Workers
{
    public class Worker : BackgroundService
    {
        private readonly RabbitMQServices _rabbitService;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public Worker(IServiceDriver mongoService, RabbitMQServices rabbitService, IHostApplicationLifetime hostApplicationLifetime)
        {
            _rabbitService = rabbitService;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("🚀 Iniciando servicio de consumo RabbitMQ...");

            try
            {
                try
                {
                    _ = _rabbitService.ConsumingRideFareReady();
                    Console.WriteLine("✅ Consumidor de RabbitMQ iniciado correctamente");
                }
                catch (Exception ex)
                {
                    await HandleWorkerErrorAsync("Error iniciando ConsumingRideFareReady", ex);
                }

                try
                {
                    _ = _rabbitService.RejectTrip();
                    Console.WriteLine("✅ RejectTrip iniciado correctamente");
                }
                catch (Exception ex)
                {
                    await HandleWorkerErrorAsync("Error iniciando RejectTrip", ex);
                }

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                await HandleWorkerErrorAsync("Error crítico en Worker", ex, isCritical: true);
            }
        }


        public async Task HandleWorkerErrorAsync(string contextMessage, Exception ex, string? IdClient = "unknown", bool isCritical = false)
        {
            // System.Console.WriteLine();
            try
            {
                await _rabbitService.PublishErrorDriverAsync(
                    IdClient: IdClient ?? "unknown",
                    MessageError: contextMessage,
                    DetailError: ex.Message,
                    Suggest: "Error de conductores intenta mas tarde",
                    CodeStatus: 500
                );
            }
            catch (Exception pubEx)
            {
                Console.WriteLine($"❌ Error al publicar el mensaje de error: {pubEx.Message}");
            }

            if (isCritical)
            {
                Console.WriteLine("🛑 Error crítico, deteniendo aplicación...");
                _hostApplicationLifetime.StopApplication();
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("🛑 Deteniendo servicio RabbitMQ...");
            _rabbitService.Dispose();
            await base.StopAsync(cancellationToken);
        }
    }
}