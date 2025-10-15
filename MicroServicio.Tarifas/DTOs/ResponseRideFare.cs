using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroServicio.Tarifas.DTOs
{
    public class ResponseRideFare
    {
        public string FareId { get; set; } = string.Empty;
        public double Price { get; set; } = 0;
        public double PricePrivate { get; set; } = 0;
        public double DistanceMax { get; set; } = 0;
        public double DistanceMin { get; set; } = 0;
        public double StopFarePrice { get; set; } = 0;
        public double MaxStopsAllowed { get; set; } = 0;
        public string locality { get; set; } = string.Empty;
        public List<string> AcceptedPaymentMethods { get; set; } = new List<string>();
        public string RequestId { get; set; }
    }
}
