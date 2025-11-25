using SGHR.Domain.Base;
using SGHR.Domain.Entities.Reservas;

namespace SGHR.Domain.Entities.Configuration
{
    public sealed class Piso : BaseEntity<int>
    {
        public int Numero { get; set; }
        public string? Descripcion { get; set; }

        public List<Habitacion> Habitaciones { get; set; } = [];

    }
}
