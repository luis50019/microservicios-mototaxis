namespace ServicioTarifas.Domain.Models;

public class PrivateFare
{
    public Guid Id { get; set; }
    public Guid FareId { get; set; }
    public double Price { get; set; }
    public bool IsActive { get; set; } = true;

    // Relación
    public Fare Fare { get; set; }
}
