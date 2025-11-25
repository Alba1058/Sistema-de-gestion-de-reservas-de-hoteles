using SGHR.Domain.Base;
using SGHR.Domain.Entities.Reservas;

namespace SGHR.Domain.Entities.Configuration
{
    public sealed class Categoria : BaseEntity<int>
    {
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public List<Habitacion> Habitaciones { get; set; } = [];

    }
}
