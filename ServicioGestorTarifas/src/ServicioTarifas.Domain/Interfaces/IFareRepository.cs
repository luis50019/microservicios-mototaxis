using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicioTarifas.Domain.Interfaces
{
    public interface IFareRepository
    {
        Task<Fare> addRideFare(Fare newFare);
        Task<Fare> getRideFare(string Id);
        Task<Fare> UpdateDistanceRideFare(string id, double? distanceMin = null, double? distanceMax = null);
        Task<Fare> UpdatePriceRideFare(string Id, double newPrice);
    }
}