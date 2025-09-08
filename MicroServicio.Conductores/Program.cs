using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using MicroServicio.Conductores.Data;
using MicroServicio.Conductores.Services;
using MicroServicio.Conductores.Interfaces;
using MicroServicio.Conductores.Config;
using MicroServicio.Conductores.Workers;

class Program
{
    static async Task Main(string[] args)
    {
        // Creamos el host
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                // Leemos appsettings.json
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                // Inyectamos la configuración de MongoDB
                services.Configure<MongoDBSettings>(
                    context.Configuration.GetSection("MongoDb"));

                // Registramos MongoDbContext
                services.AddSingleton<MongoDBContext>();

                // Registramos el servicio que hace operaciones en la DB
                services.AddSingleton<DriverService>();

                services.AddSingleton<IServiceDriver, DriverService>();

                //?añadimos RabbitMQ
                services.Configure<RabbitMQSettings>(context.Configuration.GetSection("RabbitMQ"));

                services.AddSingleton<RabbitMQServices>();

                // Registramos el Worker que estará escuchando siempre
                services.AddHostedService<Worker>();
            })
            .Build();

        // Ejecuta el host (esto deja corriendo tu Worker)
        await host.RunAsync();
    }
}
