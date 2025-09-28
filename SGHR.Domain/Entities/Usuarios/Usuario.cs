using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SGHR.Domain.Base;

namespace SGHR.Domain.Entities.Usuarios
{
    public sealed class Usuario : BaseEntity<int>
    {
        public string Nombre { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string Contrasena { get; set; } = null!;
        public int IdRolUsuario { get; set; }
    }
}