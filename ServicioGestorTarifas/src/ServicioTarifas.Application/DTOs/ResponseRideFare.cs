using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServicioTarifas.Domain;

namespace ServicioTarifas.Application.DTOs
{
    public class ResponseRideFare
    {
        public string Id { get; set; }

        public double? Price { get; set; } = 0;
        public double PricePrivate { get; set; } = 0;
        public string Locality { get; set; } = string.Empty;

        public double DistanceMin { get; set; }
        public double DistanceMax { get; set; }
        public string FareType { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}