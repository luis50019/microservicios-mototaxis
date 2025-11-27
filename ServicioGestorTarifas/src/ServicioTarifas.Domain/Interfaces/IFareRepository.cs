using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServicioTarifas.Domain.Models;

namespace ServicioTarifas.Domain.Interfaces
{
    public interface IFareRepository
    {
        Task<Fare> addRideFare(Fare newFare);
        Task<Fare> getRideFare(Guid Id);
        Task<Fare> UpdateDistanceRideFare(Guid id, double? distanceMin = null, double? distanceMax = null);
        Task<Fare> UpdatePriceRideFare(Guid Id, double newPrice);
    }
}