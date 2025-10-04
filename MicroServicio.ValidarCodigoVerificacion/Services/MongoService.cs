using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroServicio.ValidarCodigoVerificacion.Data;
using MicroServicio.ValidarCodigoVerificacion.DTOs;
using MicroServicio.ValidarCodigoVerificacion.Errors;
using MicroServicio.ValidarCodigoVerificacion.interfaces;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MicroServicio.ValidarCodigoVerificacion.Services
{
    public class MongoService : IMongoService
    {
        private readonly MongoDbContext _context;

        public MongoService(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseValidateCode> validateCode(RequestValidateCode request)
        {
            try
            {
                //*Obtenemos la reservacion
                //? debe de ser el mismo id y ademas debe de tener el mismo codigo de verificación
                var reservations = await _context.reservations.AsQueryable().Where(reservations =>
                    reservations.Id == request.idReservation && reservations.Security.CodeVerification == request.codeVerification
                ).FirstOrDefaultAsync();

                return new ResponseValidateCode
                {
                    idClient = request.idClient,
                    idReservation = reservations.Id,
                    isCorrect = reservations != null,
                    idDriver = request.idDriver,
                    Message = reservations != null?"Codigo valido":"Codigo no valido"
                };

            }
            catch (MongoException ex)
            {
                throw new ErrorMongo(ex.Message, "Error al validar el codigo");
            }   
        }
    }
}