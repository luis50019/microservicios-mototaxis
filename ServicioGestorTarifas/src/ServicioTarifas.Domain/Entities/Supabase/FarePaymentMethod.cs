namespace ServicioTarifas.Domain.Models;

public class FarePaymentMethod
{
    public Guid FareId { get; set; }
    public int PaymentMethodId { get; set; }

    // Navegación
    public Fare Fare { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
}
