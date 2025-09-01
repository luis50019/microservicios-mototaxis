using MongoDB.Driver;
using Microsoft.Extensions.Options;
using MicroServicio.Tarifas.Models;

namespace MiMicroservicio.Data
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

        public IMongoCollection<Fare> RidesFares => _database.GetCollection<Fare>("rideFares");
    }
}
