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
            Console.WriteLine("Insertando nueva reserva...");
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

            Console.WriteLine("Reserva insertada correctamente.");

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
                    SpacenNumber = 0
                },
                General = "Completado"
            });

            var filterDriver = Builders<Driver>.Filter.Eq(d => d.Id, ObjectId.Parse(request.IdDriver));
            var updateDriver = Builders<Driver>.Update
                .Set(d => d.StateDriver, "Disponible")
                .Inc(d => d.Performance.AcceptanceRate, 1)
                .CurrentDate(d => d.UpdatedAt);


            var resultDriver = await _context.drivers.UpdateOneAsync(filterDriver, updateDriver);
            var result = await _context.Reservations.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0 && resultDriver.ModifiedCount > 0;
        }

        public async Task<Boolean> RejectTrip(RejectTripDTO request)
        {
            Console.WriteLine("Rechazando la reserva..."+request.IdReservation);
            var filter = Builders<Reservation>.Filter.Eq(r => r.Id, ObjectId.Parse(request.IdReservation));
            var update = Builders<Reservation>.Update.Set(r => r.State, new State
            {
                Details = new StateDetails
                {
                    Detail = request.Details,
                    SpacenNumber = 0
                },
                General = request.General
            });

            var filterDriver = Builders<Driver>.Filter.Eq(d => d.Id, ObjectId.Parse("68ef3321a11a77b13aa17e0d"));
            var updateDriver = Builders<Driver>.Update
                .Set(d => d.StateDriver, "Disponible")
                .Inc(d => d.Performance.CanceledTrips, 1)
                .CurrentDate(d => d.UpdatedAt);

            var resultDriver = await _context.drivers.UpdateOneAsync(filterDriver, updateDriver);
            var result = await _context.Reservations.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0 && resultDriver.ModifiedCount > 0;
        }

    }
}