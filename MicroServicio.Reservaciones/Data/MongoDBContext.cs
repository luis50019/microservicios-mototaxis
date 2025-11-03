using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroServicio.Conductores.Data;
using MicroServicio.Reservaciones.Config;
using MicroServicio.Reservaciones.models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MicroServicio.Reservaciones.Data
{

    public class MongoDBContext
    {
        private readonly IMongoDatabase _database;

        //** Realizamos la conexion con la base de datos
        public MongoDBContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);//?Cadena de conexión
            _database = client.GetDatabase(settings.Value.Database);//?Nombre de la base de datos
        }

        //** Obtenemos la colecciones con las que trabajaremos
        public IMongoCollection<Reservation> RidesFares => _database.GetCollection<Reservation>("reservations");
        public IMongoCollection<Driver> drivers => _database.GetCollection<Driver>("drivers");
    }
}