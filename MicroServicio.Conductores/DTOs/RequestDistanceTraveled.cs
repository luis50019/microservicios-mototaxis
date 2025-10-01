using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroServicio.Conductores.DTOs
{
    public class RequestDistanceTraveled
    {
        public string idUser { get; set; }
        public double distanceTraveled { get; set; }
        public string typeUser { get; set; }
    }
}