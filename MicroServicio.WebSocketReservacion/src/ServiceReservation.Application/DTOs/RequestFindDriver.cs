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
    public double priceTraveled { get; set; } //*costo del viaje
    public InfoPassenger infoPassenger { get; set; }
    public string typeService { get; set; } = string.Empty;
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

  public class InfoPassenger
  {
    public string urlPhoto { get; set; }
    public string name { get; set; }
    public string phone { get; set; }
    public string lada { get; set; }
  }

  public class FareInfoResponse
  {
    public string idUser { get; set; }
    public FareInfo fareInfo { get; set; }

  }
}