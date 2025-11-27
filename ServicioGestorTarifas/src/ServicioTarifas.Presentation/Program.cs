using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServicioTarifas.Application.Interfaces;
using ServicioTarifas.Application.Services;
using ServicioTarifas.Domain.Interfaces;
using ServicioTarifas.Infrastructure.Data;
using ServicioTarifas.Infrastructure.Repositories;
using ServicioTarifas.Presentation.Controllers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

//?añadiendo el servicio y su interfaz
builder.Services.AddScoped<IRideFaresService, RideFareService>();
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

//*Conexion a mongoDbAtlas
//builder.Services.AddScoped<IFareRepository, MongoFareLocation>();

//* Añadiendo Supabase como proveedor de datos
builder.Services.AddDbContext<TarifasDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("SupabaseDb"))
);
builder.Services.AddScoped<IFareRepository, SupabaseFareLocation>();

//?Añadiendo controladores
builder.Services.AddControllers();

//?Añadiendo swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//?Añadiendo mongoDb
//builder.Services.AddMongoDb(builder.Configuration);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    //?hacemos uso de swagger
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//?añadimos los controladores
app.MapControllers();
//?ejecutamos la aplicacion
app.Run();

