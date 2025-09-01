using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicioTarifas.Application.DTOs
{
    public class RequestNewRideFare
    {
        public double distanceMax { get; set; } = 0;
        public double distamceMin { get; set; } = 0;
        public double price { get; set; } = 0;
        public int stopLimit { get; set; } = 0;
        public double stopLimitPrice { get; set; } = 0;

    }
}