using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceReservation.Application.DTOs
{
    public class RequestAcceptTrip
    {
        public InfoDriver infoDriver { get; set; }
        public InfoRideFare fareInfo { get; set; }
        public Coordinates origin { get; set; }
        public Coordinates destination { get; set; }
    }

    public class InfoDriver
    {
        public string message { get; set; } = string.Empty;
        public  Data data { get; set; }
    }

}