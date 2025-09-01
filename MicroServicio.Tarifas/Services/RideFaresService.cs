using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroServicio.Tarifas.Models;
using MiMicroservicio.Data;
using MongoDB.Driver;

namespace MicroServicio.Tarifas.Services
{
    public class RideFaresService : IMongoService
    {
        private readonly MongoDBContext _context;
        public RideFaresService(MongoDBContext context)
        {
            _context = context;
        }

        public async Task<List<Fare>> GetAllFares()
        {
            return await _context.RidesFares.Find(_ => true).ToListAsync();
        }
    }
}