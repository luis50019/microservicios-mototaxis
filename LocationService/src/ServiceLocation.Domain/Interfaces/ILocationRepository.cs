using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceLocation.Domain.Entities;

namespace ServiceLocation.Domain.Interfaces
{
    public interface ILocationRepository
    {

        //?Los metodos para obtener la informacion de un usuario solo devuelven las coordenadas
        Task<Coordinates> GetUserByIdAsync(string id);
        Task<Coordinates> GetDriverByIsAsync(string id);
        Task UpdateLocationAsyn(string id, Coordinates coordinates);
        Task UpdateDriverLocationAsync(string id, Coordinates coordinates);
    }
}