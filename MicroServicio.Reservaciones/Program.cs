using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using MicroServicio.Reservaciones.Data;
using MicroServicio.Reservaciones.Services;
using MicroServicio.Reservaciones.Config;
using MicroServicio.Reservaciones.Workers;
using MicroServicio.Reservaciones.Messages.Consumers;
using MicroServicio.Reservaciones.Messages.Producers;

class Program
{
    static async Task Main(string[] args)
    {
        // Creamos el host
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                //** Leemos appsettings.json
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                //** Inyectamos la configuración de MongoDB
                services.Configure<MongoDbSettings>(
                    context.Configuration.GetSection("MongoDb"));

                //** Registramos MongoDbContext
                services.AddSingleton<MongoDBContext>();
                services.AddSingleton<MongoService>();
                services.AddSingleton<IMongoService, MongoService>();

                //** Registramos el servicio que hace operaciones en la DB
                services.AddTransient<ReservationService>();
                services.AddTransient<ReservationCompletedTrip>();
                services.AddTransient<ReservationRejectTrip>();
                services.AddSingleton<RabbitMQRejectTrip>();
                //?añadimos RabbitMQ
                services.Configure<RabbitMQSettings>(context.Configuration.GetSection("RabbitMQ"));
                services.AddSingleton<RabbitMQService>();

                //** Registramos el servicio de errores
                services.AddSingleton<RabbitMQErrorReservation>();

                //** añadimos lso servicios consumidores
                services.AddSingleton<RabbitMQReservationConsumer>();
                services.AddSingleton<RabbitMQReservationProducers>();
                services.AddSingleton<RabbitMQCompletedTripConsumer>();

                // Registramos el Worker que estará escuchando siempre
                services.AddHostedService<Worker>();
                services.AddHostedService<WorkercompletedTrip>();
                services.AddHostedService<WorkerRejectTrip>();
            })
            .Build();

        // Ejecuta el host (esto deja corriendo tu Worker)
        await host.RunAsync();
    }
}