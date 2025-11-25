using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGHR.Domain.Entities.Usuarios;

namespace SGHR.Persistence.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> entity)
        {
            entity.ToTable("Usuario");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Nombre)
                  .IsRequired()
                  .HasMaxLength(150);

        }
    }
}
