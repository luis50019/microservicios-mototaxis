namespace ServicioTarifas.Domain.Models;

public class StopFare
{
    public Guid Id { get; set; }
    public Guid FareId { get; set; }
    public double PricePerStop { get; set; }
    public int MaxStopsAllowed { get; set; } = 5;
    public bool IsActive { get; set; } = true;

    // Relación
    public Fare Fare { get; set; }
}
