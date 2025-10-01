using SGHR.Domain.Entities.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Application.Validators
{
    public class RolUsuarioValidator
    {
        public bool Validate(RolUsuario rolUsuario, out string errorMessage)
        {
            if (!ValidationHelper.NotNull(rolUsuario, "RolUsuario", out errorMessage))
                return false;

            if (!ValidationHelper.Required(rolUsuario.Nombre, "Nombre del rol", out errorMessage))
                return false;

            if (!ValidationHelper.MaxLength(rolUsuario.Descripcion, 100, "Descripción", out errorMessage))
                return false;

            if (rolUsuario.Estado == null)
            {
                errorMessage = "El estado del rol debe estar definido.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }
}