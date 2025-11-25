using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGHR.Domain.Entities.Reservas;


namespace SGHR.Persistence.Configurations
{
    public class HabitacionConfiguration : IEntityTypeConfiguration<Habitacion>
    {
        public void Configure(EntityTypeBuilder<Habitacion> entity)
        {
            entity.ToTable("Habitacion");

            entity.Property(h => h.IdCategoria).HasColumnName("IdCategoria");
            entity.Property(h => h.IdPiso).HasColumnName("IdPiso");

            entity.HasOne(h => h.Categoria)
                  .WithMany(c => c.Habitaciones)
                  .HasForeignKey(h => h.IdCategoria)
                  .IsRequired();

            entity.HasOne(h => h.Piso)
                  .WithMany(p => p.Habitaciones)
                  .HasForeignKey(h => h.IdPiso)
                  .IsRequired();

            entity.Property(e => e.PrecioBase)
                  .HasColumnType("decimal(10, 2)");
        }
    }
}
