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
    }

    public class FareInfo
    {
        public string FareId { get; set; } = string.Empty;
        public double Price { get; set; } = 0;
        public double PricePrivate { get; set; } = 0;
        public double StopFarePrice { get; set; } = 0;
        public double MaxStopsAllowed { get; set; } = 0;
        public double DistanceMax { get; set; } = 0;
        public double DistanceMin { get; set; } = 0;
        public string Locality { get; set; } = string.Empty;

        public List<string> AcceptedPaymentMethods { get; set; } = new List<string>();
        public string RequestId { get; set; }

    }

    public class newCoordinates
    {
        public string Lat { get; set; }
        public string Lng { get; set; }
    }

    public class Fare
    {
        public string FareId { get; set; }
        public double price { get; set; }
        public double stopFarePrice { get; set; }
        public double maxStopsAllowed { get; set; }
        public string locality { get; set; } = string.Empty;
        public double ditanceMax { get; set; }
        public double distanceMax { get; set; }
        public List<string> acceptedPaymentMethods { get; set; }
        public string RequestId { get; set; }
    }
    public class infoDriver
    {
        public string message { get; set; } = string.Empty;
        public Data data { get; set; }

    }

    public class DataRquest
    {

        public string id { get; set; } = string.Empty;
        public Coordinates locationStart { get; set; } = new Coordinates();
        public Coordinates locationEnd { get; set; } = new Coordinates();
        public double priceTraveled { get; set; } //*costo del viaje
        public string client { get; set; } = string.Empty;
        public InfoPassenger infoPassager { get; set; } = new InfoPassenger();
        public Coordinates coordinates { get; set; } = new Coordinates();
        public InfoRideFare rideFare { get; set; } = new InfoRideFare();
        public string typeService { get; set; } = string.Empty;

    }

    public class InfoPassenger
  {
    public string urlPhoto { get; set; }
    public string name { get; set; }
    public string phone { get; set; }
    public string lada { get; set; }
  }

    public class Data
    {
        public string id { get; set; } = string.Empty;
        public Coordinates locationStart { get; set; } = new Coordinates();
        public Coordinates locationEnd { get; set; } = new Coordinates();
        public double priceTraveled { get; set; } //*costo del viaje
        public string client { get; set; } = string.Empty;
        public InfoPassenger infoPassager { get; set; } = new InfoPassenger();
        public Coordinates coordinates { get; set; } = new Coordinates();
        public InfoRideFare rideFare { get; set; } = new InfoRideFare();
        public string typeService { get; set; } = string.Empty;
    }



    //*! clases que no usamos al recibir la informcion del viaje
    public class RequestFindDriver
    {
        public Coordinates locationStart { get; set; }
        public Coordinates locationEnd { get; set; }
        public double priceTraveled { get; set; } //*costo del viaje
        public InfoRideFare fare { get; set; } = new InfoRideFare();
    }

    public class InfoRideFare
    {
        public string idUser { get; set; } = string.Empty;// id del cliente que solicita el viaje
        public FareInfo fareinfo { get; set; } = new FareInfo();
    }
}