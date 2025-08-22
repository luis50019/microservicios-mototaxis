using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceLocation.Application.Interfaces;
using ServiceLocation.Domain.Entities;
using ServiceLocation.Domain.Interfaces;

namespace ServiceLocation.Application.Services
{
    public class LocationServices : ILocationService
    {

        private readonly ILocationRepository _locationRepository;

        public LocationServices(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }
        public async Task<List<User>> GetAllUserAsync()
        {
            return await _locationRepository.GetAllUSerAsync();
        }

        public Task<User> GetUserByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateLocationAsync(string id, Coordinates coordinates)
        {
            throw new NotImplementedException();
        }
    }
}