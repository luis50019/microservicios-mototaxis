using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceLocation.Domain.Entities;

namespace ServiceLocation.Application.DTOs
{
    public class ResponseLocation
    {
        public string id { get; set; } = string.Empty;
        public double? Lat { get; set; } = 0;
        public double? Lng { get; set; } = 0;
    }
}