using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroServicio.Conductores.Data;
using MicroServicio.Reservaciones.models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MicroServicio.Reservaciones.Data
{
   public class MongoDBSettings
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
    }

    public class MongoDBContext
    {
        private readonly IMongoDatabase _database;

        public MongoDBContext(IOptions<MongoDBSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.Database);
        }

        public IMongoCollection<Reservation> RidesFares => _database.GetCollection<Reservation>("reservations");

        public IMongoCollection<Driver> drivers => _database.GetCollection<Driver>("drivers");
    }
}