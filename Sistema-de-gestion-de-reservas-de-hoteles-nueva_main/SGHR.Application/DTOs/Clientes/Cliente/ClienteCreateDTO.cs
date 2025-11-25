
using System.ComponentModel.DataAnnotations;

namespace SGHR.Application.DTOs.Clientes.Cliente
{
    public class ClienteCreateDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = null!;
        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string Apellido { get; set; } = null!;
        [Required(ErrorMessage = "El email es obligatorio.")] 
        [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
        public string Email { get; set; } = null!;
        [Phone(ErrorMessage = "El número de teléfono no es válido.")]
        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        public string Telefono { get; set; } = null!;
        public string? Direccion { get; set; }
        [Required(ErrorMessage = "La identificación es obligatoria.")]
        public string Identificacion { get; set; } = null!;
    }
}
