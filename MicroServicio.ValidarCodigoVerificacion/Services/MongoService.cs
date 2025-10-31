using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroServicio.ValidarCodigoVerificacion.Data;
using MicroServicio.ValidarCodigoVerificacion.DTOs;
using MicroServicio.ValidarCodigoVerificacion.Errors;
using MicroServicio.ValidarCodigoVerificacion.interfaces;
using MicroServicio.ValidarCodigoVerificacion.Models.MicroServicio.Reservaciones.models;
using MongoDB.Bson;
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
                var reservation = await _context.reservations
                    .Find(res=> res.Id == request.idReservation && res.Security.CodeVerification == request.codeVerification)
                    .FirstOrDefaultAsync();

                if (reservation == null)
                {
                    return CreateErrorResponse(request, "Código no válido");
                }

                return new ResponseValidateCode
                {
                    idClient = request.idClient,
                    idReservation = reservation.Id.ToString(),
                    idDriver = request.idDriver,
                    isCorrect = true,
                    Message = "Código válido"
                };
            }
            catch (MongoException ex)
            {
                throw new ErrorMongo(ex.Message, "Error al validar el código");
            }
            catch (Exception ex)
            {
                throw new ErrorMongo(ex.Message, "Error interno del sistema");
            }
        }

        private ResponseValidateCode CreateErrorResponse(RequestValidateCode request, string message)
        {
            return new ResponseValidateCode
            {
                idClient = request.idClient,
                idReservation = request.idReservation,
                idDriver = request.idDriver,
                isCorrect = false,
                Message = message
            };
        }

    }
}