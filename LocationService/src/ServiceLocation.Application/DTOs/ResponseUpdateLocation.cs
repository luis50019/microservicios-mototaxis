using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceLocation.Application.DTOs
{
    public class ResponseUpdateLocation
    {
        public string id { get; set; }
        public double? lat { get; set; } = 0;
        public double? lng { get; set; } = 0;
        
    }
}