using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceReservation.Application.DTOs
{
    public class RequestDistanceTraveled
    {
        public string IdUser { get; set; } = string.Empty;
        public double distanceTraveled { get; set; } = 0;
        public string typeUser { get; set; } = string.Empty;
    }

}