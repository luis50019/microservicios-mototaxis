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
            try
            {
                Console.WriteLine("🚀 Iniciando servicio de consumo RabbitMQ...");

                // Iniciar consumo UNA SOLA VEZ
                _rabbitService.ConsumingRideFareReady();
                _rabbitService.AcceptedTrip();
                _rabbitService.RejectTrip();
                Console.WriteLine("✅ Consumidor de RabbitMQ iniciado correctamente");
                
                // Mantener el worker activo sin bloquear
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error crítico en Worker: {ex.Message}");
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