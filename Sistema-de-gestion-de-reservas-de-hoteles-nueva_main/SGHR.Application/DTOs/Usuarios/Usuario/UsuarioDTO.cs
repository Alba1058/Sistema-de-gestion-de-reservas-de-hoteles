
namespace SGHR.Application.DTOs.Usuarios.Usuario
{
    public class UsuarioDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int RolUsuarioId { get; set; }
        public string RolNombre { get; set; } = null!;
        public bool Activo { get; set; }
    }
}
