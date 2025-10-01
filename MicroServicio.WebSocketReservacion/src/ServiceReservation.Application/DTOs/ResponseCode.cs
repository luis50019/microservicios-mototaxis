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
        public string IdDriver { get; set; }
    }
}