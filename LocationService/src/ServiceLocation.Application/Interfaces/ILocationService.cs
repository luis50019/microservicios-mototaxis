using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceLocation.Domain.Entities;

namespace ServiceLocation.Application.Interfaces
{
    public interface ILocationService
    {
        Task<List<User>> GetAllUserAsync();
        Task<User> GetUserByIdAsync(string id);
        Task UpdateLocationAsync(string id, Coordinates coordinates);
    }
}