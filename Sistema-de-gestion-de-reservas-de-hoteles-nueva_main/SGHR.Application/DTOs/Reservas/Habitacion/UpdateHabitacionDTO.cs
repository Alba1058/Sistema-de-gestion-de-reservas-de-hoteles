
using System.ComponentModel.DataAnnotations;

namespace SGHR.Application.DTOs.Reservas.Habitacion
{
    public class UpdateHabitacionDTO
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public int IdCategoria { get; set; }
        public int IdPiso { get; set; }
        public int EstadoHabitacion { get; set; }
        public decimal PrecioBase { get; set; }
        [MaxLength(250)]
        public string? Descripcion { get; set; }
        public bool Estado { get; set; }
    }
}