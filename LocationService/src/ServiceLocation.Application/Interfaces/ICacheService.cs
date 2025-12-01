using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceLocation.Application.DTOs;
using ServiceLocation.Domain.Entities;

namespace ServiceLocation.Application.Interfaces
{
    public interface ICacheService
    {
        
    Task<UserRedis?> SaveConnecttion(RequestUpdateLocation updateLocationm,string connectionId);
    Task<UserRedis?> Disconnect(RequestUpdateLocation location);
    Task<UserRedis?> UpdateLocation(RequestUpdateLocation location);
    }
}