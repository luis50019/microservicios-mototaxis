using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceReservation.Application.DTOs
{
    public class ResponseNotificateDriver
    {
        public string IdRireFare { get; set; } = string.Empty;
        public double distanceTraveled { get; set; } = 0;
        public double priceTraveled { get; set; } = 0;
        
    }
}