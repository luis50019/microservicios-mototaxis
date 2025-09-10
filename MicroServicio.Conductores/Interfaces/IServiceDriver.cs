using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroServicio.Conductores.Data;

namespace MicroServicio.Conductores.Interfaces
{
    public interface IServiceDriver
    {
        Task<object> FindAvailableStateAsync(Coordinates pickupLocation);//* busca un conductor
        Task<string> AcceptRideAsync(string id);//* marca al conductor como ocupado
        Task<object> RejectRideAsync(string id);//* marca al conductor como disponible

    }
}