using MongoDB.Driver;
using MicroServicio.Tarifas.Models;
using System.Threading.Tasks;

public interface IMongoService
{
    Task<bool> ExisteViaje(string idViaje);
    Task GuardarCodigoVerificacion(string idViaje, string codigo);
}

public class MongoService : IMongoService
{
    private readonly IMongoCollection<Reservation> _reservas;

    public MongoService(MongoDBContext context)
    {
        _reservas = context.Reservations; 
    }

    public async Task<bool> ExisteViaje(string idViaje)
    {
        var reserva = await _reservas.Find(r => r.Id == idViaje).FirstOrDefaultAsync();
        return reserva != null;
    }

    public async Task GuardarCodigoVerificacion(string idViaje, string codigo)
    {
        var filter = Builders<Reservation>.Filter.Eq(r => r.Id, idViaje);
        var update = Builders<Reservation>.Update.Set(r => r.VerificationCode, codigo);
        await _reservas.UpdateOneAsync(filter, update);
    }
}
