using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using ServiceReservation.Application.Interfaces;

namespace ServiceReservation.Application.Events
{
    public class ReservationPrivateEventHandler : IReservationPrivateHandler
    {
        //?añadimos el hub
        public Task HandlerAsync(ReservationEvent reservationEvent)
        {
            throw new NotImplementedException();
        }
    }
    public record ReservationEvent(string ReservationId, string status);
}