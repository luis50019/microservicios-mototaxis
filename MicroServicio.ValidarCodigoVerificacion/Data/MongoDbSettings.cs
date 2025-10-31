using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroServicio.ValidarCodigoVerificacion.Config;
using MicroServicio.ValidarCodigoVerificacion.Models.MicroServicio.Reservaciones.models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MicroServicio.ValidarCodigoVerificacion.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.Database);
        }

        public IMongoCollection<Reservation> reservations => _database.GetCollection<Reservation>("reservationsPrivate");

    }

}