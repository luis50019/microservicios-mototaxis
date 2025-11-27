namespace ServicioTarifas.Domain.Models;

public class CustomFare
{
    public Guid Id { get; set; }
    public Guid FareId { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public double Price { get; set; }
    public string[] ApplicableDays { get; set; }
    public bool IsActive { get; set; } = true;

    // Relación
    public Fare Fare { get; set; }
}
