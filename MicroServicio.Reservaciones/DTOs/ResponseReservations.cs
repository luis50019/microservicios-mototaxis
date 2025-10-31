using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroServicio.Reservaciones.DTOs
{
    public class ResponseReservation
    {
        public string IdClient { get; set; } = string.Empty;
        public string IdDriver { get; set; } = string.Empty;
        public string IdReservation { get; set; } = string.Empty;
        public string CodeVerification { get; set; } = string.Empty;
        public InfoDriver InfoDriver { get; set; } = new();
    }

    public class InfoDriver
    {
        public string idDriver { get; set; }
        public string name { get; set; }
        public string PhotoDriver { get; set; }
        public string LicensePlate { get; set; }
        public string Phone { get; set; }
        public double? numberUnit { get; set; } = 0;
    }
}