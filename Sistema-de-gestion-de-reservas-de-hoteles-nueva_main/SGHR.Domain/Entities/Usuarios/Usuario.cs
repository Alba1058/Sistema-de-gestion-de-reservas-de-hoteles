using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;

namespace SGHR.Domain.Entities.Usuarios
{
    public sealed class Usuario : BaseEntity<int>
    {
        public string Nombre { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Contrasena { get; set; } = null!;
        public int RolUsuarioId { get; set; }
        public RolUsuario RolUsuario { get; set; } = null!;
    }
}