using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroServicio.CodigoVerificacion.Configurations;
using MicroServicio.CodigoVerificacion.models;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using MicroServicio.Conductores.Data;

namespace MicroServicio.CodigoVerificacion.Data
{
    public class MongoDBContext
    {
        private readonly IMongoDatabase _database;

        public MongoDBContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.Database);
        }

        public IMongoCollection<Reservation> Reservations
        {
            get
            {
                return _database.GetCollection<Reservation>("Reservations");
            }
        }
        public IMongoCollection<Driver> Drivers
        {
            get
            {
                return _database.GetCollection<Driver>("drivers");
            }
        }
    }
}
