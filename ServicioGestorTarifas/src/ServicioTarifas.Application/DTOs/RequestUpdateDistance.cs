using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicioTarifas.Application.DTOs
{
    public class RequestUpdateDistance
    {
        public double? DistanceMin { get; set; }
        public double? DistanceMax { get; set; }
    }
}