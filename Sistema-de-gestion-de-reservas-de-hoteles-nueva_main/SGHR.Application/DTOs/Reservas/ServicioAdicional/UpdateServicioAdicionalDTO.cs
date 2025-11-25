
using System.ComponentModel.DataAnnotations;

namespace SGHR.Application.DTOs.Reservas.ServicioAdicional
{
    public class UpdateServicioAdicionalDTO
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Nombre { get; set; } = null!;
        public decimal Precio { get; set; }
        [MaxLength(250)]
        public string? Descripcion { get; set; }
        public bool Estado { get; set; }
    }
}