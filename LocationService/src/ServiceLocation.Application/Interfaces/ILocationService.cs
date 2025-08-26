using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceLocation.Application.DTOs;
using ServiceLocation.Domain.Entities;

namespace ServiceLocation.Application.Interfaces
{
    public interface ILocationService
    {
        Task<ResponseLocation> GetUserByIdAsync(string id,string typeUser);
        Task UpdateLocationAsync(string id,string typeUser, Coordinates coordinates);
    }
}