﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using MicroServicio.Tarifas.Services;
using MicroServicio.CodigoVerificacion.Data;
using MicroServicio.CodigoVerificacion.Configurations;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        // configuraciones
        services.Configure<MongoDbSettings>(context.Configuration.GetSection("MongoDb"));
        services.Configure<RabbitMQSettings>(context.Configuration.GetSection("RabbitMQ"));

        //sservicios
        services.AddSingleton<MongoDBContext>();
        services.AddSingleton<IMongoService, MongoService>();
        services.AddSingleton<RabbitMQService>();

        //Worker de fondo
        services.AddHostedService<ReservationWorker>();
    })
    .Build();

await host.RunAsync();