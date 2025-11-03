using System;
using System.Threading;
using System.Threading.Tasks;
using MicroServicio.Tarifas.Services;
using Microsoft.Extensions.Hosting;

namespace MicroServicio.Tarifas.Workers
{
    public class Worker : BackgroundService
    {
        private readonly IMongoService _mongoService;
        private readonly RabbitMQService _rabbitService;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public Worker(IMongoService mongoService, RabbitMQService rabbitService, IHostApplicationLifetime hostApplicationLifetime)
        {
            _mongoService = mongoService;
            _rabbitService = rabbitService;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                Console.WriteLine("🚀 Iniciando servicio de consumo RabbitMQ...");

                // Iniciar consumo UNA SOLA VEZ
                _rabbitService.StartConsuming();

                Console.WriteLine("✅ Consumidor de RabbitMQ iniciado correctamente");

                // Mantener el worker activo sin bloquear
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (MongoException ex)
            {
                //! Error de mongoDB
                Console.WriteLine($"❌ Error de conexión a MongoDB en Worker: {ex.Message}");
                throw new ErrorMongo(ex.Message, "Error al iniciar el servicio de tarifas. Verifica la conexión con la base de datos.");
            }
            catch (BrokerUnreachableException ex)
            {
                //! Error con RabbitMQ
                Console.WriteLine($"❌ Error de conexión a RabbitMQ en Worker: {ex.Message}");
                _hostApplicationLifetime.StopApplication();
            }
            catch (Exception ex)
            {
                //! otro error inesperado
                Console.WriteLine($"❌ Error crítico en Worker: {ex}");
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