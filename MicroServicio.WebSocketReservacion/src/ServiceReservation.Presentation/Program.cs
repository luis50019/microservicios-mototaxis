using ServiceReservation.Infrastructure.Configurations;
using ServiceReservation.Infrastructure.Messaging;
using ServiceReservation.Infrastructure.Messaging.Consumers;
using ServiceReservation.Infrastructure.Messaging.Producers;
using ServiceReservation.Presentation.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
//!añadimos swaager para el mapeo de las rutas
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//!añadimos signalR
builder.Services.AddSignalR();
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000);
});

//!configuramos RabbitMQ
builder.Services.Configure<RabbitMqSettings>(
    builder.Configuration.GetSection("RabbitMQ")
);

//!Añadimos el servicio de RabbitMQ
builder.Services.AddSingleton<RabbitMqService>();

//!Añadimos todos los consumidores y publicadores de RabbitMQ
builder.Services.AddSingleton<RabbitMqRideFarePublisher>();
builder.Services.AddSingleton<RabbitMQFindDriver>();
builder.Services.AddSingleton<RabbitMqRideFareConsumer>();
builder.Services.AddSingleton<RabbitMQDriverConsumer>();
builder.Services.AddSingleton<RabbitMQCodeSecurity>();

//!Añadimos el userconnectionManager

builder.Services.AddSingleton<UserConnectionManager>();

//builder.Services.AddSingleton<>();
//!Anadimos los controladores
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

//!Le indicamos que use el hub
app.MapHub<ReservationHub>("/reservations");

app.UseHttpsRedirection();
app.Run();

