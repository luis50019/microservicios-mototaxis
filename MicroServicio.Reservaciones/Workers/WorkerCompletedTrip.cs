using System;
using System.Threading;
using System.Threading.Tasks;
using MicroServicio.Reservaciones.Services;
using Microsoft.Extensions.Hosting;

namespace MicroServicio.Reservaciones.Workers
{
    public class WorkercompletedTrip : BackgroundService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly ReservationCompletedTrip _service;
        public WorkercompletedTrip(ReservationCompletedTrip reservationService, IHostApplicationLifetime hostApplicationLifetime)
        {
            _service = reservationService;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                Console.WriteLine("Iniciando servicio de consumo RabbitMQ...");
                await _service.StartCompletedTripAsync();
                Console.WriteLine("Consumidor de RabbitMQ iniciado correctamente");

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error crítico en Worker viaje completado: {ex.Message}");
                _hostApplicationLifetime.StopApplication();
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("🛑 Deteniendo servicio RabbitMQ...");
            _service.Dispose();
            await base.StopAsync(cancellationToken);
        }
    }
}