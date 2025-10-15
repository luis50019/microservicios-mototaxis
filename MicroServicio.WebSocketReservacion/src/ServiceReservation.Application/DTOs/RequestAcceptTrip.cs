using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceReservation.Application.DTOs
{
    public class RequestAcceptTrip
    {
        public InfoDriver infoDriver { get; set; }
    }

    public class InfoDriver
    {
        public string message { get; set; } = string.Empty;
        public  Data data { get; set; }
    }

}