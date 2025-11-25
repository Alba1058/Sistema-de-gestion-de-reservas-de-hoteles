
using System.ComponentModel.DataAnnotations;

namespace SGHR.Application.DTOs.Clientes.Cliente
{
    public class ClienteUpdateDTO
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public string? Direccion { get; set; }
        public string Identificacion { get; set; } = null!;
    }
}
