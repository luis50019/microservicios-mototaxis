using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using MicroServicio.Tarifas.Services;
using MicroServicio.Tarifas.Data;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        // configuraciones
        services.Configure<MongoDBSettings>(context.Configuration.GetSection("MongoDb"));
        services.Configure<RabbitMQSettings>(context.Configuration.GetSection("RabbitMQ"));

        //sservicios
        services.AddSingleton<MongoDBContext>();
        services.AddSingleton<IMongoService, MongoService>();
        services.AddSingleton<IRabbitMQService, RabbitMQService>();

        //Worker de fondo
        services.AddHostedService<ReservationWorker>();
    })
    .Build();

await host.RunAsync();
