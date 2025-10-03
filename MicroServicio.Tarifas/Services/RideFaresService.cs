using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroServicio.Tarifas.DTOs;
using MicroServicio.Tarifas.Models;
using MiMicroservicio.Data;
using MongoDB.Driver.Linq;
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

        //TODO: método que buscara la tarifa por medio de la distancia a recorrer|
        public async Task<ResponseRideFare?> GetRideFareAsync(double distanceTraveled,string locality)
        {
        
            var fare = await _context.RidesFares
                    .AsQueryable()
                    .Where(f => distanceTraveled >= f.DistanceMin && distanceTraveled <= f.DistanceMax
                    && f.locality == locality && f.IsActive == true)
                    .FirstOrDefaultAsync();
            if (fare == null)
            {
                Console.WriteLine("entro qui");
                return null;
            }
            return new ResponseRideFare
            {
                FareId = fare.Id,
                Price = fare.FareType.Global.Price,
                DistanceMin = fare.DistanceMin,
                DistanceMax = fare.DistanceMax,
                StopFarePrice = fare.StopFare.PricePerStop,
                MaxStopsAllowed = fare.StopFare.MaxStopsAllowed,
                AcceptedPaymentMethods = fare.AcceptedPaymentMethods,
                PricePrivate = fare.FareType.Private.Price,
                locality = fare.locality
            };
        }
    }

}