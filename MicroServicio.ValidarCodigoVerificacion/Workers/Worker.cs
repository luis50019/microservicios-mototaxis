using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroServicio.ValidarCodigoVerificacion.Services;
using Microsoft.Extensions.Hosting;

namespace MicroServicio.ValidarCodigoVerificacion.Workers
{
    public class Worker : BackgroundService
    {
        private readonly ValidateCodeService _service;
        private readonly IHostApplicationLifetime _hostAplicationLifeTime;

        public Worker(ValidateCodeService service, IHostApplicationLifetime hostApplicationLifetime)
        {
            _service = service;
            _hostAplicationLifeTime = hostApplicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _service.StartAsync();
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en el worker: {ex.Message}");
                _hostAplicationLifeTime.StopApplication();
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _service.Dispose();
            await base.StartAsync(cancellationToken);
        }
  }
}