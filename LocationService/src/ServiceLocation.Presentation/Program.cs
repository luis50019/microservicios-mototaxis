using ServiceLocation.Application.Interfaces;
using ServiceLocation.Application.Services;
using ServiceLocation.Infrastructure.Data.Mongo;
using ServiceLocation.Infrastructure.Data.Redis;
using ServiceLocation.Presentation.HubLocation;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

//?Añadiendo servicios de aplicacion
builder.Services.AddScoped<ILocationService, LocationServices>();
builder.Services.AddScoped<ICacheService, ConnectionUserService>();

//!Añadiendo controladores
builder.Services.AddControllers();

//?Añadiendo swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5260);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactNative", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed((host) => true); // permite todas las IPs (solo para pruebas)
    });
});


//*Añadimos SignalR
builder.Services.AddSignalR();

//?Añadiendo MongoDb
builder.Services.AddMongoDb(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowReactNative");
app.MapHub<LocationHub>("/locations");
app.Run();
