using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServicioTarifas.Application.DTOs;

namespace ServicioTarifas.Application.Interfaces
{
    public interface IRideFaresService
    {

        //!Por el momento este metodo solo recibira la distancia max y min, ademas el precio, el limite de paradas y el precio de cada parada 
        Task<ResponseRideFare> AddRideFare(RequestNewRideFare newFare);
        Task<ResponseRideFare> GetRideFare(string Id);
        Task<ResponseRideFare> UpdateDistanceAsync(string id, double? distanceMin = null, double? distanceMax = null);
        Task<ResponseRideFare> UpdatePriceAsync(string Id, double newPrice);
    }
}