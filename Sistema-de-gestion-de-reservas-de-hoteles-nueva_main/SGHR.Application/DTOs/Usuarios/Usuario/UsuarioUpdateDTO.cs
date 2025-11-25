
using System.ComponentModel.DataAnnotations;

namespace SGHR.Application.DTOs.Usuarios.Usuario
{
    public class UsuarioUpdateDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;
        public bool Activo { get; set; }
        [Required]
        public int RolUsuarioId { get; set; }
    }
}
