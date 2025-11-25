
using System.ComponentModel.DataAnnotations;

namespace SGHR.Application.DTOs.Reservas.Tarifa
{
    public class UpdateTarifaDTO
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Tipo { get; set; } = null!;
        public decimal Monto { get; set; }
        public DateOnly FechaInicio { get; set; }
        public DateOnly FechaFin { get; set; }
        public decimal PrecioPorNoche { get; set; }
        public decimal Descuento { get; set; }
        [MaxLength(250)]
        public string Descripcion { get; set; } = null!;
        public int IdHabitacion { get; set; }
        public bool Estado { get; set; }
    }
}
