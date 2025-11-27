using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServicioTarifas.Application.DTOs;
using ServicioTarifas.Application.Exceptions;
using ServicioTarifas.Application.Interfaces;
using ServicioTarifas.Domain;
using ServicioTarifas.Domain.Interfaces;
using ServicioTarifas.Domain.Models;

namespace ServicioTarifas.Application.Services
{
    public class RideFareService : IRideFaresService
    {

        private readonly IFareRepository _rideFareRepository;

        public RideFareService(IFareRepository repository)
        {
            _rideFareRepository = repository;
        }
        public async Task<ResponseRideFare> AddRideFare(RequestNewRideFare newFare)
        {
            //?generamos una nueva tarifa
            try
            {
                var newRideFare = new Fare
                {
                    Locality = newFare.nameLocality,
                    DistanceMax = newFare.distanceMax,
                    DistanceMin = newFare.distamceMin,
                    FareType = newFare.type == "private" ? FareType.Private :
                    newFare.type == "stop" ? FareType.Stop :
                                          FareType.Global,
                    Price = newFare.price,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                Fare result = await _rideFareRepository.addRideFare(newRideFare);

                return new ResponseRideFare
                {
                    Id = result.Id.ToString(),
                    CreatedAt = result.CreatedAt,
                    Locality = result.Locality,
                    DistanceMax = result.DistanceMax,
                    DistanceMin = result.DistanceMin,
                    Price = result.Price,
                    FareType = result.FareType.ToString(),
                    IsActive = result.IsActive
                };

            }
            catch (Exception ex)
            {
                throw new ExceptionRideFare("erro al agregar la nueva tarifa",
                new
                {
                    erro = "No se logro agregar la tarifa",
                    detailt = ex.Message
                });

            }
        }

        //TODO: metodo para obtener una tarifa por su id
        public async Task<ResponseRideFare> GetRideFare(string Id)
        {
            try
            {
                var result = await _rideFareRepository.getRideFare(Guid.Parse(Id));
                if (result == null)
                {
                    throw new Exception("La tarifa no fue encontrda");
                }

                return new ResponseRideFare
                {
                    Id = result.Id.ToString(),
                    FareType  = result.FareType.ToString(),
                    CreatedAt = result.CreatedAt,
                    DistanceMax = result.DistanceMax,
                    DistanceMin = result.DistanceMin,
                    Price = result.Price,
                    IsActive = result.IsActive
                };
            }
            catch (Exception ex)
            {

                throw new ExceptionRideFare("Error de tarifa",
                new
                {
                    error = "Error al obtener la informacion de la tarifa",
                    detail = ex.Message
                });
            }
        }

        //TODO: metodo para actualizar la distancia de una tarifa
        public async Task<ResponseRideFare> UpdateDistanceAsync(string id, double? distanceMin = null, double? distanceMax = null)
        {
            try
            {
                var result = await _rideFareRepository.UpdateDistanceRideFare(Guid.Parse(id), distanceMin, distanceMax);
                if (result == null)
                {
                    throw new Exception("No se logro actualizar la tarifa");
                }

                return new ResponseRideFare
                {
                    Id = result.Id.ToString(),
                    CreatedAt = result.CreatedAt,
                    DistanceMax = result.DistanceMax,
                    DistanceMin = result.DistanceMin,
                    FareType = result.FareType.ToString(),
                    Price = result.Price,
                    IsActive = result.IsActive
                };
            }
            catch (Exception ex)
            {

                throw new ExceptionRideFare("Error de tarifa",
                new
                {
                    error = "Error al actualizar la distancia de la tarifa",
                    detail = ex.Message
                });
            }
        }

        //TODO: metodo para actualizar el precio de una tarifa

        public async Task<ResponseRideFare> UpdatePriceAsync(string Id, double newPrice)
        {
            try
            {
                var result = await _rideFareRepository.UpdatePriceRideFare(Guid.Parse(Id), newPrice);
                if (result == null)
                {
                    throw new Exception("No se logro actualizar la tarifa");
                }

                return new ResponseRideFare
                {
                    Id = result.Id.ToString(),
                    CreatedAt = result.CreatedAt,
                    DistanceMax = result.DistanceMax,
                    DistanceMin = result.DistanceMin,
                    FareType = result.FareType.ToString(),
                    Price = result.Price,
                    IsActive = result.IsActive
                };
            }
            catch (Exception ex)
            {

                throw new ExceptionRideFare("Error de tarifa",
                new
                {
                    error = "Error al actualizar el precio de la tarifa",
                    detail = ex.Message
                });
            }
        }
    }
}