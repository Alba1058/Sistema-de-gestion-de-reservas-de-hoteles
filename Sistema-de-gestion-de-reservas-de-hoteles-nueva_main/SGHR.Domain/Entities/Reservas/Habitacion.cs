using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Domain.Enums;

namespace SGHR.Domain.Entities.Reservas
{
    public sealed class Habitacion : BaseEntity<int>
    {
        public int Numero { get; set; }

        public int IdCategoria { get; set; }
        public Categoria Categoria { get; set; } = null!;
        public Piso Piso { get; set; } = null!;
        public int IdPiso { get; set; }
        public EstadoHabitacion EstadoH { get; set; }
        public decimal PrecioBase { get; set; }
        public string? Descripcion { get; set; }
        public List<Reserva> Reservas { get; set; } = [];

        public ICollection<HabitacionServicio> HabitacionServicios { get; set; } = [];
    }
}