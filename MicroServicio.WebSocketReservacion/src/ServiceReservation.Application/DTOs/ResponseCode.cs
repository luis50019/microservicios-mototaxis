using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceReservation.Application.DTOs
{
    public class ResponseCode
    {
        public string Code { get; set; }
        public string IdViaje { get; set; }
        public string IdClient { get; set; }
        public infoDriverCode DataDriver { get; set; }
    }
    public class infoDriverCode
    {
        public string idDriver { get; set; }
        public string name { get; set; }
        public double rating { get; set; }
        public string PhotoDriver { get; set; }
        public string LicensePlate { get; set; }
        public string Phone { get; set; }
        public double? numberUnit { get; set; } = 0;
    }
}