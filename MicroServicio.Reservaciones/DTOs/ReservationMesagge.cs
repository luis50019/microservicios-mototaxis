using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroServicio.Reservas.DTOs
{
    public class ReservationMessage
    {
        public string IdReservation { get; set; } = string.Empty;
        public string IdClient { get; set; } = string.Empty;
        public string IdDriver { get; set; } = string.Empty;
        public string IdRideFare { get; set; } = string.Empty;
        public double Distance { get; set; }
    }
}