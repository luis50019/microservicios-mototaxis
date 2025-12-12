using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceLocation.Domain.Entities;

namespace ServiceLocation.Application.DTOs
{
    public class RequestUpdateLocation
    {
        public string Id { get; set; } = string.Empty; //! Id del usuarios que se conecto
        public string? IdClient { get; set; } = string.Empty;//! id del conductor o del usaurio       
        public string TypeUser { get; set; } = string.Empty;
        public Coordinates newLocation { get; set; } = new Coordinates { Lat = 0, Lng = 0 };
    }
}