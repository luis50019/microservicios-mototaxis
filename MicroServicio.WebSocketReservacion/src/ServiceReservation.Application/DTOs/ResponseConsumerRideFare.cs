


namespace ServiceReservation.Application.DTOs
{

  public class ResponseConsumerRideFare
  {
    public string IdUser { get; set; } = string.Empty;
    public FareInfo Fare { get; set; } = new FareInfo();
    public string ErrorMessage { get; set; } = string.Empty;


  }

  public class FareInfo
  {
    public string FareID { get; set; } = string.Empty;
    public double Price { get; set; } = 0;
    public double StopFarePrice { get; set; } = 0;
    public double MaxStopsAllowed { get; set; } = 0;
    public double DistanceMax { get; set; } = 0;
    public double DistanceMin { get; set; } = 0;

    public List<string> AcceptedPaymentMethods { get; set; } = new List<string>();

  }


}
