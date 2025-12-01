using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceLocation.Domain.Entities;

namespace ServiceLocation.Domain.Interfaces
{
    public interface IUserRespository
    {
        //** Metodo para guardar la informacion del usuario conectado */
        Task<UserRedis?> SaveUserConnected(string Id, string TypeUser, Coordinates location,string connectionId);
        
        Task<UserRedis?> DisconnectUser(string Id, string TypeUser,Coordinates location);

        //** Metodo para establecer la ubicación del usuario  o conductor*/
        Task <UserRedis?> SetLocationUser(string Id,Coordinates locationUpdate,string IdClient);
    }
}