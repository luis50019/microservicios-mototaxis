using MicroServicio.Tarifas.DTOs;
using MiMicroservicio.Data;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using MicroServicio.Tarifas.Errors;

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
        public async Task<ResponseRideFare> GetRideFareAsync(double distanceTraveled, string locality)
        {
            try
            {
                var fare = await _context.RidesFares
                        .AsQueryable()
                        .Where(f => distanceTraveled >= f.DistanceMin && distanceTraveled <= f.DistanceMax
                        && f.locality == locality && f.IsActive == true)
                        .FirstOrDefaultAsync();
                if (fare == null)
                {
                    throw new CustomError(
                        MessageError: "No se encontró una tarifa válida para la distancia o localidad especificada.",
                        DetailError: $"Distancia: {distanceTraveled}, Localidad: {locality}",
                        Suggest: "Verifica que existan tarifas configuradas para esa zona o rango de distancia.",
                        IdClient: "system",
                        CodeStatus: 404
                    );
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
            catch (MongoException ex)
            {
                throw new CustomError(
                    MessageError: "Error al conectarse con la base de datos.",
                    DetailError: ex.Message,
                    Suggest: "Verifica la conexión a MongoDB o el estado del servidor.",
                    IdClient: "system",
                    CodeStatus: 500
                );
            }
            catch (CustomError)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CustomError(
                    MessageError: "Error inesperado al calcular la tarifa.",
                    DetailError: ex.Message,
                    Suggest: "Contacta con el administrador del sistema.",
                    IdClient: "system",
                    CodeStatus: 500
                );
            }
        }
    }

}