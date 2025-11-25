
using System.ComponentModel.DataAnnotations;

namespace SGHR.Application.DTOs.Configuration.RolUsuario
{
    public class CreateRolUsuarioDTO
    {
        [Required, MaxLength(100)]
        public string Nombre { get; set; } = null!;
        [MaxLength(250)]
        public string? Descripcion { get; set; }
        public bool Estado { get; set; } = true;
    }
}