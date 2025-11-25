using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGHR.Domain.Entities.Reservas;
using SGHR.Domain.Enums;


namespace SGHR.Persistence.Configurations
{
    public class ReservaConfiguration : IEntityTypeConfiguration<Reserva>
    {
        public void Configure(EntityTypeBuilder<Reserva> entity)
        {

            entity.ToTable("Reserva");

            entity.Property(r => r.IdCliente).HasColumnName("IdCliente");
            entity.Property(r => r.IdHabitacion).HasColumnName("IdHabitacion");

            entity.HasOne(r => r.Cliente)
                  .WithMany(c => c.Reservas)
                  .HasForeignKey(r => r.IdCliente)
                  .IsRequired();

            entity.HasOne(r => r.Habitacion)
                  .WithMany(h => h.Reservas)
                  .HasForeignKey(r => r.IdHabitacion)
                  .IsRequired();

            entity.Property(e => e.EstadoReserva)
                  .HasConversion<int>()
                  .HasDefaultValue(EstadoReserva.Activa);

            entity.Property(e => e.Total)
                  .HasColumnType("decimal(10, 2)");


        }
    }
}
