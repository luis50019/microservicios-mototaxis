using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroServicio.Conductores.Data;

namespace MicroServicio.Conductores.DTOs
{
    public class RequestRideFareReady
    {
        public string idUser { get; set; }
        public Coordinates locationStart { get; set; }
        public Coordinates locationEnd { get; set; }
        public double priceTraveled { get; set; }
        
    }
}