var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
//!añadimos swaager para el mapeo de las rutas
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//!Anadimos los controladores
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//!Le indicamos que mape los controladores

app.UseHttpsRedirection();
app.Run();

