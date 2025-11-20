using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceReservation.Application.DTOs
{
    public class RequestRejectTrip
    {
        public string IdReservation { get; set; } = string.Empty;
        public string IdDriver { get; set; } = string.Empty;
        public string IdClient { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string General { get; set; } = string.Empty;
    }
}