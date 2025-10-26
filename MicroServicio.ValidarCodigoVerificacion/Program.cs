using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MicroServicio.ValidarCodigoVerificacion.Config;
using MicroServicio.ValidarCodigoVerificacion.Data;
using MicroServicio.ValidarCodigoVerificacion.Services;
using MicroServicio.ValidarCodigoVerificacion.interfaces;
using MicroServicio.ValidarCodigoVerificacion.Messages.Publisher;
using MicroServicio.ValidarCodigoVerificacion.Messages.Consumers;
using MicroServicio.ValidarCodigoVerificacion.Workers;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        // Agregar appsettings.json explícitamente si quieres
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        // Registrar servicios
        builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDb"));
        builder.Services.AddSingleton<MongoDbContext>();

        builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));
        builder.Services.AddSingleton<RabbitMQService>();
        builder.Services.AddSingleton<RabbitMQValidateCodeConsumer>();
        builder.Services.AddSingleton<RabbitMQValidateCodePublisher>();
        builder.Services.AddSingleton<MongoService>();
        builder.Services.AddSingleton<ValidateCodeService>();
        builder.Services.AddSingleton<IMongoService, MongoService>();

        // Registrar Worker si lo tienes
        builder.Services.AddHostedService<Worker>();

        var host = builder.Build();

        await host.RunAsync();
    }
}
