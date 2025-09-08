using System;
using System.Threading.Tasks;
using MicroServicio.Conductores.Data;
using MicroServicio.Conductores.Interfaces;
using MongoDB.Driver;
using MongoDB.Bson;
using MicroServicio.Conductores.DTOs;

namespace MicroServicio.Conductores.Services
{
    public class DriverService : IServiceDriver
    {
        private readonly MongoDBContext _context;
        public DriverService(MongoDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Conductor acepta un viaje. Cambia el estado a "Ocupado".
        /// </summary>
        public async Task<object> AcceptRideAsync(string driverId)
        {
            var filter = Builders<Driver>.Filter.Eq(d => d.Id, ObjectId.Parse(driverId));
            var update = Builders<Driver>.Update
                .Set(d => d.StateDriver, "Ocupado")
                .Inc(d => d.Performance.AcceptanceRate, 1) // ajusta tasa de aceptación
                .CurrentDate(d => d.UpdatedAt);

            var result = await _context.Drivers.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0 ? "Viaje aceptado" : "Conductor no encontrado";
        }

        /// <summary>
        /// Encuentra un conductor disponible cerca del punto de recogida.
        /// </summary>
        public async Task<object> FindAvailableStateAsync(Coordinates pickupLocation)
        {
            // 📍 Para algo real, deberías usar un cálculo de distancia (Haversine).
            var filter = Builders<Driver>.Filter.Eq(d => d.StateDriver, "Disponible");
            var driver = await _context.Drivers.Find(filter).FirstOrDefaultAsync();

            if (driver == null)
                return "No hay conductores disponibles";

            return new
            {
                driver.Id,
                driver.BasicInfo.Name,
                driver.Unit,
                driver.Location.Current.Coordinates
            };
        }

        /// <summary>
        /// Simula asignar un conductor a un cliente (encontrado en cola principal).
        /// </summary>
        public async Task<DriverFound> FoundConductorAsync()
        {
            var filter = Builders<Driver>.Filter.Eq(d => d.StateDriver, "Disponible");
            var driver = await _context.Drivers.Find(filter).FirstOrDefaultAsync();

            if (driver == null)
                return new DriverFound
                {
                    succes = false,
                    id = driver.Id.ToString(),
                    coordinates = driver.Location.Current.Coordinates,
                    infoBasic = driver.BasicInfo,
                    unit = driver.Unit,
                    State = "No hay conductores disponibles"
                    
                }; 

            // Cambiamos a estado "En espera de aceptación"
            var update = Builders<Driver>.Update
                .Set(d => d.StateDriver, "EnEspera")
                .CurrentDate(d => d.UpdatedAt);

            await _context.Drivers.UpdateOneAsync(d => d.Id == driver.Id, update);

            return new DriverFound
            {
                succes = true,
                id = driver.Id.ToString(),
                coordinates = driver.Location.Current.Coordinates,
                infoBasic = driver.BasicInfo,
                unit = driver.Unit,
                State = "Conductor encontrado, esperando aceptación"
                
            };
        }

        /// <summary>
        /// Conductor rechaza el viaje. Estado vuelve a "Disponible".
        /// </summary>
        public async Task<object> RejectRideAsync(string driverId)
        {
            var filter = Builders<Driver>.Filter.Eq(d => d.Id, ObjectId.Parse(driverId));
            var update = Builders<Driver>.Update
                .Set(d => d.StateDriver, "Disponible")
                .Inc(d => d.Performance.CanceledTrips, 1) // estadísticas
                .CurrentDate(d => d.UpdatedAt);

            var result = await _context.Drivers.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0 ? "Viaje rechazado" : "Conductor no encontrado";
        }
    }
}
