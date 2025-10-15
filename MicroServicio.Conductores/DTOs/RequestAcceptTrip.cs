using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroServicio.Conductores.Data;

namespace MicroServicio.Conductores.DTOs
{
    public class RequestAcceptTrip
    {
        public infoDriver infoDriver { get; set; } = new infoDriver();
        public FareInfo fareInfo { get; set; } = new FareInfo();
    }

    public class FareInfo
    {
        public string idUser { get; set; } = string.Empty;
        public Fare fareinfo { get; set; } = new Fare();
    }

    public class Fare {
        public string FareId { get; set; } = string.Empty;
        public double price { get; set; }
        public double stopFarePrice { get; set; }
        public double maxStopsAllowed { get; set; }
        public string locality { get; set; } = string.Empty;
        public double ditanceMax { get; set; }
        public double distanceMax { get; set; }
        public List<string> acceptedPaymentMethods { get; set; } = new List<string>();
        public string RequestId { get; set; }
     }
    public class infoDriver
    {
        public string message { get; set; } = string.Empty;
        public Data data { get; set; } = new Data();

    }
    public class Data {
        public string id { get; set; } = string.Empty;
        public string client { get; set; } = string.Empty;
        public Coordinates coordinates { get; set; } = new Coordinates();
    }
}