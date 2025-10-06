using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroServicio.Conductores.Data;

namespace MicroServicio.Conductores.DTOs
{
    public class RequestRideFareReady
    {
        public string idUser { get; set; } = string.Empty;
        public Coordinates locationStart { get; set; } = new Coordinates();
        public Coordinates locationEnd { get; set; } = new Coordinates();
        public double priceTraveled { get; set; }
        
    }
}