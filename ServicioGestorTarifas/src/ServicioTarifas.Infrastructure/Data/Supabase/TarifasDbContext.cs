using Microsoft.EntityFrameworkCore;
using ServicioTarifas.Domain.Models;

namespace ServicioTarifas.Infrastructure.Data
{
    public class TarifasDbContext : DbContext
    {
        public TarifasDbContext(DbContextOptions<TarifasDbContext> options)
            : base(options)
        {
        }

        public DbSet<Fare> Fares { get; set; }
        public DbSet<GlobalFare> GlobalFares { get; set; }
        public DbSet<PrivateFare> PrivateFares { get; set; }
        public DbSet<StopFare> StopFares { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<FarePaymentMethod> FarePaymentMethods { get; set; }
        public DbSet<CustomFare> CustomFares { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // PK compuesta para tabla intermedia N:N
            modelBuilder.Entity<FarePaymentMethod>()
                .HasKey(fpm => new { fpm.FareId, fpm.PaymentMethodId });

            // Opcional: relaciones explícitas
            modelBuilder.Entity<FarePaymentMethod>()
                .HasOne(fpm => fpm.Fare)
                .WithMany(f => f.FarePaymentMethods)
                .HasForeignKey(fpm => fpm.FareId);

            modelBuilder.Entity<FarePaymentMethod>()
                .HasOne(fpm => fpm.PaymentMethod)
                .WithMany(pm => pm.FarePaymentMethods)
                .HasForeignKey(fpm => fpm.PaymentMethodId);
        }
    }
}
