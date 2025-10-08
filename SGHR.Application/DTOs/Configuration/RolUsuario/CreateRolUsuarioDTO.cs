using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Application.DTOs.Configuration.RolUsuario
{
    public class CreateRolUsuarioDTO
    {
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
    }
}