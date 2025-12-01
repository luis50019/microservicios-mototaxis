using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceLocation.Domain.Entities;

namespace ServiceLocation.Application.DTOs
{
    public class RequestUpdateLocation
    {
        public string Id { get; set; } = string.Empty;
        public string? IdClient { get; set; } = string.Empty;       
        public string TypeUser { get; set; } = string.Empty;
        public Coordinates newLocation { get; set; } = new Coordinates { Lat = 0, Lng = 0 };
    }
}