namespace ServicioTarifas.Domain.Models;

public class Fare
{
    public Guid Id { get; set; }
    public string Locality { get; set; }
    public double DistanceMin { get; set; }
    public double Price { get; set; }
    public double DistanceMax { get; set; }
    public FareType FareType { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Relaciones
    public ICollection<GlobalFare> GlobalFares { get; set; }
    public ICollection<PrivateFare> PrivateFares { get; set; }
    public ICollection<StopFare> StopFares { get; set; }
    public ICollection<CustomFare> CustomFares { get; set; }
    public ICollection<FarePaymentMethod> FarePaymentMethods { get; set; }
}

public enum FareType
{
    Global,
    Private,
    Stop
}

