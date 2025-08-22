using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using ServiceLocation.Domain.Entities;
using ServiceLocation.Domain.Interfaces;

namespace ServiceLocation.Infrastructure.Repositories
{
    public class MongoLocationRepository : ILocationRepository
    {
        private readonly IMongoCollection<User> _userCollection;
        private readonly IMongoCollection<Driver> _driverCollection;

        public MongoLocationRepository(IMongoDatabase database)
        {
            _userCollection = database.GetCollection<User>("users");
            _driverCollection = database.GetCollection<Driver>("drivers");
        }

        public async Task<List<User>> GetAllUSerAsync()
        {
            Console.WriteLine("llego al repo de mongo");
            return await _userCollection.Find(_ => true).ToListAsync();
        }

        public Task<User> GetUserByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateLocationAsyn(string id, Coordinates coordinates)
        {
            throw new NotImplementedException();
        }
    }
}