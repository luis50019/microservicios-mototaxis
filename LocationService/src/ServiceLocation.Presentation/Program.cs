using ServiceLocation.Application.Interfaces;
using ServiceLocation.Application.Services;
using ServiceLocation.Infrastructure.Data.Mongo;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

//?Añadiendo servicios de aplicacion
builder.Services.AddScoped<ILocationService, LocationServices>();

//!Añadiendo controladores
builder.Services.AddControllers();

//?Añadiendo swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//?Añadiendo MongoDb
builder.Services.AddMongoDb(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
