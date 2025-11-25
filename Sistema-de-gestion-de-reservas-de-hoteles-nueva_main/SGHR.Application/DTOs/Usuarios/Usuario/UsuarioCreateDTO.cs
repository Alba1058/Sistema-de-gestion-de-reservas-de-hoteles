
using System.ComponentModel.DataAnnotations;

namespace SGHR.Application.DTOs.Usuarios.Usuario
{
    public class UsuarioCreateDTO
    {
        [Required]
        public int RolUsuarioId { get; set; }
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        public string Nombre { get; set; } = null!;
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        public string Email { get; set; } = null!;
        public string Contrasena { get; set; } = null!;
    }
}
