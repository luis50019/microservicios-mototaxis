using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ServicioTarifas.Infrastructure.Data;

public class TarifasDbContextFactory : IDesignTimeDbContextFactory<TarifasDbContext>
{
    public TarifasDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TarifasDbContext>();

        // Cadena directa para migraciones
        optionsBuilder.UseNpgsql(
            "Host=aws-1-us-east-2.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.miylyqyrgzmyyzgqziem;Password=SyndicateControl23481;SSL Mode=Require;Trust Server Certificate=true;Pooling=true;Maximum Pool Size=10;"
        );

        return new TarifasDbContext(optionsBuilder.Options);
    }
}
