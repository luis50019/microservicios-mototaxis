using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using ServicioTarifas.Domain;
using ServicioTarifas.Domain.Interfaces;

namespace ServicioTarifas.Infrastructure.Repositories
{
    public class MongoFareLocation : IFareRepository
    {

        //?Coleccion donde se alamcenan las tarifas
        private readonly IMongoCollection<Fare> _rideFareCollection;

        public MongoFareLocation(IMongoDatabase database)
        {
            _rideFareCollection = database.GetCollection<Fare>("rideFares");
        }

        //*metodo para agregar una nueva tarifa
        //?recibe la nueva tarifa
        public async Task<Fare> addRideFare(Fare newFare)
        {
            await _rideFareCollection.InsertOneAsync(newFare);
            return newFare;
        }


        //*metodo para obtener una tarifa por su id
        //?recibe el id de la tarifa a obtener
        public async Task<Fare> getRideFare(string Id)
        {
            return await _rideFareCollection.Find(
                fare => fare.Id == Id
            ).FirstOrDefaultAsync();
        }


        //*metodo para actualizar la distancia minima y maxima de una tarifa
        //?recibe el id de la tarifa y la distancia minima y maxina
        public async Task<Fare> UpdateDistanceRideFare(string id, double? distanceMin = null, double? distanceMax = null)
        {
            var updateDef = Builders<Fare>.Update;
            var updates = new List<UpdateDefinition<Fare>>();

            if (distanceMin.HasValue) updates.Add(updateDef.Set(f => f.DistanceMin, distanceMin.Value));
            if (distanceMax.HasValue) updates.Add(updateDef.Set(f => f.DistanceMax, distanceMax.Value));

            if (updates.Count == 0) return await getRideFare(id);

            updates.Add(updateDef.Set(f => f.LastUpdated, DateTime.UtcNow));

            var result = await _rideFareCollection.UpdateOneAsync(f => f.Id == id, updateDef.Combine(updates));
            if (result.MatchedCount == 0) return null;

            return await getRideFare(id);
        }


        //*metodo para actualiza el precio de una tarifa
        //?recibe el id de la tarifa y el nuevo precio de la tarifa
        public async Task<Fare> UpdatePriceRideFare(string Id, double newPrice)
        {
            var update = Builders<Fare>.Update.Set(f => f.FareType.Global.Price, newPrice).Set(f => f.LastUpdated, DateTime.UtcNow);

            var updateFare = await _rideFareCollection.UpdateOneAsync(f => f.Id == Id, update);
            if (updateFare.MatchedCount == 0) return null;

            return await getRideFare(Id);

        }
    }

}