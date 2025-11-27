using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServicioTarifas.Domain;
using ServicioTarifas.Domain.Interfaces;
using ServicioTarifas.Domain.Models;
using ServicioTarifas.Infrastructure.Data;

namespace ServicioTarifas.Infrastructure.Repositories
{
    public class SupabaseFareLocation : IFareRepository
    {

        private readonly TarifasDbContext _context;
        public SupabaseFareLocation(TarifasDbContext context)
        {
            _context = context;
        }
        public async Task<Fare> addRideFare(Fare newFare)
        {
            newFare.Id = Guid.NewGuid();
            newFare.CreatedAt = DateTime.UtcNow;
            newFare.UpdatedAt = DateTime.UtcNow;

            _context.Fares.Add(newFare);
           await _context.SaveChangesAsync();

            return newFare;
        }

        public async Task<Fare> getRideFare(Guid Id)
        {
            return await _context.Fares.AsNoTracking().FirstOrDefaultAsync(f => f.Id == Id);
        }

        public async Task<Fare> UpdateDistanceRideFare(Guid id, double? distanceMin = null, double? distanceMax = null)
        {

            var fare = await _context.Fares.FindAsync(id);

            if (fare == null)
                return null;

            if (distanceMin.HasValue)
                fare.DistanceMin = distanceMin.Value;

            if (distanceMax.HasValue)
                fare.DistanceMax = distanceMax.Value;

            fare.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return fare;
        }

        public async Task<Fare> UpdatePriceRideFare(Guid Id, double newPrice)
        {

            var fare = await _context.Fares.FindAsync(Id);

            if (fare == null)
                return null;

            // Asumimos que el precio pertenece a tabla GlobalFare
            var globalFare = await _context.GlobalFares
                .FirstOrDefaultAsync(g => g.FareId == fare.Id);

            if (globalFare == null)
            {
                globalFare = new GlobalFare
                {
                    FareId = fare.Id,
                    Price = newPrice,
                    IsActive = true
                };

                _context.GlobalFares.Add(globalFare);
            }
            else
            {
                globalFare.Price = newPrice;
                globalFare.IsActive = true;
            }

            fare.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return fare;
        }
    }
}