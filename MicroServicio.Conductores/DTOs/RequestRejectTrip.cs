using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroServicio.Conductores.DTOs
{
    public class RequestRejectTrip
    {
        public string idDriver { get; set; } = string.Empty;
        public string idClient { get; set; } = string.Empty;
    }
}