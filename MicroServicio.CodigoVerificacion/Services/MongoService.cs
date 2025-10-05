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
        var reserva = await _reservas.Find(r => r.Id == idViaje).FirstOrDefaultAsync();
        return reserva != null;
    }

    public async Task<InfoDriver> GuardarCodigoVerificacion(string idViaje, string codigo, string idDriver)
    {
        var filter = Builders<Reservation>.Filter.Eq(r => r.Id, idViaje);
        var update = Builders<Reservation>.Update.Set(r => r.Security.CodeVerification, codigo);
        await _reservas.UpdateOneAsync(filter, update);
        var filterDriver = Builders<Driver>.Filter.Eq(f => f.Id, ObjectId.Parse(idDriver));
        var driver = await _drivers.Find(filterDriver).FirstOrDefaultAsync();

        return new InfoDriver
        {
            idDriver = idDriver,
            LicensePlate = driver.Unit.LicensePlate,
            Phone = driver.BasicInfo.Phone.Number,
            PhotoDriver = driver.BasicInfo.ProfilePicture,
            numberUnit = driver.Unit.Number == null?0:driver.Unit.Number
        };
        
    }
}