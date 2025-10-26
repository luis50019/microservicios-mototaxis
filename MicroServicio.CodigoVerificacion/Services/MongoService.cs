using MicroServicio.CodigoVerificacion.Data;
using MicroServicio.CodigoVerificacion.models;
using MicroServicio.Conductores.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;

public interface IMongoService
{
    Task<bool> ExisteViaje(string idViaje);
    Task<InfoDriver> GuardarCodigoVerificacion(string idViaje, string codigo, string idDriver);
}

public class MongoService : IMongoService
{
    private readonly IMongoCollection<Reservation> _reservas;
    private readonly IMongoCollection<Driver> _drivers;

    public MongoService(MongoDBContext context)
    {
        _reservas = context.Reservations;
        _drivers = context.Drivers;
    }

    public async Task<bool> ExisteViaje(string idViaje)
    {
        var reserva = await _reservas.Find(r => r.Id == ObjectId.Parse(idViaje)).FirstOrDefaultAsync();

        return reserva == null;
    }

    public async Task<InfoDriver> GuardarCodigoVerificacion(string idViaje, string codigo, string idDriver)
    {
        Console.WriteLine("id Viaje:" + idViaje.ToString());

        var filter = Builders<Reservation>.Filter.Eq(r => r.Id, ObjectId.Parse(idViaje));

        var update = Builders<Reservation>.Update
            .Set(r => r.Security, new Security
            {
                CodeVerification = codigo,
                IsVerified = false
            })
            .CurrentDate(r => r.UpdatedAt);

        var result = await _reservas.UpdateOneAsync(filter, update);

        Console.WriteLine($"Matched: {result.MatchedCount}, Modified: {result.ModifiedCount}");



        var filterDriver = Builders<Driver>.Filter.Eq(f => f.Id, ObjectId.Parse(idDriver));
        var driver = await _drivers.Find(filterDriver).FirstOrDefaultAsync();

        return new InfoDriver
        {
            idDriver = idDriver.ToString(),
            LicensePlate = driver.Unit.LicensePlate,
            name = driver.BasicInfo.Name,
            Phone = driver.BasicInfo.Phone.Number,
            PhotoDriver = driver.BasicInfo.ProfilePicture,
            numberUnit = driver.Unit.Number == null ? 0 : driver.Unit.Number
        };

    }
}