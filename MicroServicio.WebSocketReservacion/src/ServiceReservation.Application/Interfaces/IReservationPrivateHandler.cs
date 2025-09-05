using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceReservation.Application.Events;

namespace ServiceReservation.Application.Interfaces
{
    public interface IReservationPrivateHandler
    {
        Task HandlerAsync(ReservationEvent reservationEvent);
    }
}