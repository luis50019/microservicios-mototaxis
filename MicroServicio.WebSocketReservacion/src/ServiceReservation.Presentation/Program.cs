using Microsoft.AspNetCore.SignalR;
using ServiceReservation.Infrastructure.Configurations;
using ServiceReservation.Infrastructure.Messaging;
using ServiceReservation.Infrastructure.Messaging.Consumers;
using ServiceReservation.Infrastructure.Messaging.Producers;
using ServiceReservation.Presentation.Hubs;
using ServiceReservation.Presentation.Listeners;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
//!añadimos swaager para el mapeo de las rutas
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactNative", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed(origin => true);
    });
});

//!añadimos signalR
builder.Services.AddSignalR();

//!configuramos RabbitMQ
builder.Services.Configure<RabbitMqSettings>(
    builder.Configuration.GetSection("RabbitMQ")
);

//!Añadimos todos los consumidores y publicadores de RabbitMQ
builder.Services.AddSingleton<RabbitMqService>();
builder.Services.AddSingleton<UserConnectionManager>();
builder.Services.AddSingleton<RabbitMqRideFarePublisher>();
builder.Services.AddSingleton<RabbitMQFindDriver>();
builder.Services.AddSingleton<RabbitMqRideFareConsumer>();
builder.Services.AddSingleton<RabbitMQDriverConsumer>();
builder.Services.AddSingleton<RabbitMQCodeSecurity>();
builder.Services.AddSingleton<RabbitMqValidateCodePublisher>();

//! ocupamos singleton para poder ejecutar los listener de las respuestas de rabbit
builder.Services.AddHostedService<FareResponseListener>();
builder.Services.AddHostedService<DriverListener>();
builder.Services.AddHostedService<CodeVerificationListener>();
builder.Services.AddHostedService<ValidateCodeListener>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseWebSockets();
app.UseRouting();
app.UseCors("AllowReactNative");
//!Le indicamos que use el hub
app.MapHub<ReservationHub>("/reservations");

app.Run();

