using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroServicio.Conductores.Data;
using MicroServicio.Reservaciones.models;

namespace MicroServicio.Reservaciones.DTOs
{
    public class RequestReservations
    {
        public infoDriver infoDriver { get; set; }
        public FareInfo fareInfo { get; set; }
        public Coordinates origin { get; set; }
        public Coordinates destination { get; set; }
    }

    public class FareInfo
    {
        public string idUser { get; set; }
        public Fare fareinfo { get; set; }
    }
    
    public class newCoordinates {
        public string Lat { get; set; }
        public string Lng { get; set; }
    }

    public class Fare
    {
        public string FareId { get; set; }
        public double price { get; set; }
        public double stopFarePrice { get; set; }
        public double maxStopsAllowed { get; set; }
        public double ditanceMax { get; set; }
        public double distanceMax { get; set; }
        public List<string> acceptedPaymentMethods { get; set; }
    }
    public class infoDriver
    {
        public string message { get; set; } = string.Empty;
        public Data data { get; set; }

    }
    public class Data {
        public string id { get; set; } = string.Empty;
        public string client { get; set; } = string.Empty;
        public newCoordinates coordinates { get; set; }
    }
}