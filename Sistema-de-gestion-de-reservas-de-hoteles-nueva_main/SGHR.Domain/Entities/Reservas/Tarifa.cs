
namespace SGHR.Domain.Entities.Reservas
{
    using SGHR.Domain.Base;

    public sealed class Tarifa : BaseEntity<int>
    {

        public string Tipo { get; set; } = null!;
        public decimal Monto { get; set; }
        public DateOnly FechaInicio { get; set; }
        public DateOnly FechaFin { get; set; }
        public decimal PrecioPorNoche { get; set; }
        public decimal Descuento { get; set; }
        public required string Descripcion { get; set; }
        public int IdHabitacion { get; set; }
        public Habitacion Habitacion { get; set; } = null!;
    }
}