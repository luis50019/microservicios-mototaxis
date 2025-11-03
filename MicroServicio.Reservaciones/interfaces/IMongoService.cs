using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroServicio.Reservaciones.DTOs;

namespace MicroServicio.Reservaciones.Services
{
    public interface IMongoService
    {
        public Task<ResponseReservation> Insert(RequestReservations request);
    }
}