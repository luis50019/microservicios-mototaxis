using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceLocation.Domain.Entities;

namespace ServiceLocation.Domain.Interfaces
{
    public interface ILocationRepository
    {
        Task<List<User>> GetAllUSerAsync();
        Task<User> GetUserByIdAsync(string id);
        Task UpdateLocationAsyn(string id, Coordinates coordinates);
    }
}