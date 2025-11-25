using SGHR.Domain.Base;
using SGHR.Domain.Entities.Reservas;
using SGHR.Domain.Entities.Usuarios;

namespace SGHR.Domain.Entities.Configuration
{
    public sealed class RolUsuario : BaseEntity<int>
    {
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }

       
    }
}
