using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceReservation.Application.DTOs
{
    public class RequestFinishTrip
    {
        public string IdReservation { get; set; }
        public string IdDriver { get; set; }
        public string IdClient { get; set; }
        public string Details { get; set; }
        public string General { get; set; }
    }
}