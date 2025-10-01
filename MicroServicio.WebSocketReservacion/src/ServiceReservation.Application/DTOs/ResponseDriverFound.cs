using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceReservation.Application.DTOs
{

    public class ResponseDriverFound
    {
        public string Event { get; set; } = string.Empty;
        public Data Data { get; set; } = new Data();    
    }
    public class Data
    {
        public string id { get; set; } = string.Empty;
        public string client { get; set; } = string.Empty;
        public Coordinates coordinates { get; set; } = new Coordinates();
    }

}
