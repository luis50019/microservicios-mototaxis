namespace ServicioTarifas.Domain.Models;

public class GlobalFare
{
    public Guid Id { get; set; }
    public Guid FareId { get; set; }
    public double Price { get; set; }
    public bool IsActive { get; set; }

    // Relación
    public Fare Fare { get; set; }
}
