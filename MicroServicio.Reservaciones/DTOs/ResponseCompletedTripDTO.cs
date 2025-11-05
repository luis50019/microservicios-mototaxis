using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroServicio.Reservaciones.DTOs
{
    public class ResponseCompletedTripDTO
    {
        public string IdClient { get; set; }
        public string IdDriver { get; set; }
        public string Message { get; set; }
        public bool StateUpdate { get; set; }
    }
}