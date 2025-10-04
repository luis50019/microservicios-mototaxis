using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceReservation.Application.DTOs
{
    public class RequestFindDriver
    { 
        public Coordinates locationStart { get; set; }
        public Coordinates locationEnd { get; set; }
        public double priceTraveled { get; set; }
        public InfoRideFare fare { get; set; } = new InfoRideFare();
    }

    public class InfoRideFare
    {
        public string idUser { get; set; } = string.Empty;// id del cliente que solicita el viaje
        public FareInfo fareinfo { get; set; } = new FareInfo();    
    }

     public class Coordinates
    {
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }
}