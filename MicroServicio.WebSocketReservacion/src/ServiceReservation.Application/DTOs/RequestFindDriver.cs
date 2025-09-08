using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceReservation.Application.DTOs
{
    public class RequestFindDriver
    {
        public string idUser { get; set; }
        public Coordinates locationStart { get; set; }
        public Coordinates locationEnd { get; set; }
        public double priceTraveled { get; set; }
    }

     public class Coordinates
    {
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }
}