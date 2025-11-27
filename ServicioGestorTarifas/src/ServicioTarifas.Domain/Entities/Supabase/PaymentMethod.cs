namespace ServicioTarifas.Domain.Models;

public class PaymentMethod
{
    public int Id { get; set; }
    public string Name { get; set; }

    // Relación para Many-to-Many
    public ICollection<FarePaymentMethod> FarePaymentMethods { get; set; }
}
