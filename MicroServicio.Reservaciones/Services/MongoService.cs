using MicroServicio.Conductores.Data;
using MicroServicio.Reservaciones.Data;
using MicroServicio.Reservaciones.DTOs;
using MicroServicio.Reservaciones.Errors;
using MicroServicio.Reservaciones.models;
using MicroServicio.Reservaciones.useCases.create;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MicroServicio.Reservaciones.Services
{
    public class MongoService : IMongoService
    {
        public readonly MongoDBContext _context;
        public MongoService(MongoDBContext context)
        {
            _context = context;
        }
        public async Task<ResponseReservation> Insert(RequestReservations request)
        {
            Console.WriteLine("Iniciando registro de reservación...");
            var newReservation = ReservationCase.CreateReservation(request);
            //* insertamos la nueva tarifa
            await _context.Reservations.InsertOneAsync(newReservation);
            //?Acutalizamos el estado del conductor a "Ocupado" y aumentamos su tasa de aceptación
            var filter = Builders<Driver>.Filter.Eq(d => d.Id, ObjectId.Parse(request.infoDriver.data.id));
            var update = Builders<Driver>.Update
                .Set(d => d.StateDriver, "Ocupado")
                .Inc(d => d.Performance.AcceptanceRate, 1)
                .CurrentDate(d => d.UpdatedAt);
            var result = await _context.drivers.UpdateOneAsync(filter, update);
            var driver = await _context.drivers.Find(d => d.Id == ObjectId.Parse(request.infoDriver.data.id))
                          .FirstOrDefaultAsync();

            if (driver == null) throw new ErrorMongoService(404, "Conductor no encontrado en la base de datos.");
            return ReservationCase.CreateResponseReservation(newReservation, driver);
        }

        public async Task<Boolean> CompletedTrip(CompletedTripDTO request)
        {
            var filter = Builders<Reservation>.Filter.Eq(r => r.Id, ObjectId.Parse(request.IdReservation));
            var update = Builders<Reservation>.Update.Set(r => r.State, new State
            {
                Details = new StateDetails
                {
                    Detail = "Se llego al destino de forma correcta",
                    SpacenNumber = request.SpacenNumber
                },
                General = "Completado"
            });

            var result = await _context.Reservations.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }


    }
}