using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGHR.Domain.Entities.Reservas;

namespace SGHR.Persistence.Configurations
{
    public class ServicioAdicionalConfiguration : IEntityTypeConfiguration<ServicioAdicional>
    {
        public void Configure(EntityTypeBuilder<ServicioAdicional> builder)
        {
            builder.ToTable("ServicioAdicional");
            builder.HasKey(s => s.Id);

        }
    }
}