using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using global::SGHR.Domain.Entities.Usuarios;


namespace SGHR.Domain.Validators
{
    public class UsuarioValidator
    {
        public bool Validate(Usuario usuario, out string errorMessage)
        {
            if (!ValidationHelper.NotNull(usuario, "Usuario", out errorMessage))
                return false;

            if (!ValidationHelper.Required(usuario.Nombre, "Nombre", out errorMessage))
                return false;

            if (!ValidationHelper.Required(usuario.Email, "Correo", out errorMessage))
                return false;

            if (!usuario.Email.Contains("@"))
            {
                errorMessage = "El correo electrónico no es válido.";
                return false;
            }

            if (!ValidationHelper.Required(usuario.Contrasena, "Contraseña", out errorMessage))
                return false;

            if (!ValidationHelper.GreaterThanZero(usuario.RolUsuarioId, "IdRolUsuario", out errorMessage))
                return false;

            errorMessage = string.Empty;
            return true;
        }
    }
}