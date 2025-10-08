using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SGHR.Domain.Entities.Clientes;


namespace SGHR.Domain.Validators
{
    public class ClienteValidator
    {
            public bool Validate(Cliente cliente, out string errorMessage)
            {
                if (!ValidationHelper.NotNull(cliente, "Cliente", out errorMessage))
                    return false;

                if (!ValidationHelper.Required(cliente.Nombre, "Nombre", out errorMessage))
                    return false;

                if (!ValidationHelper.Required(cliente.Apellido, "Apellido", out errorMessage))
                    return false;

                if (!ValidationHelper.Required(cliente.Telefono, "Teléfono", out errorMessage))
                    return false;

                if (!ValidationHelper.Required(cliente.Email, "Email", out errorMessage))
                    return false;

                if (!cliente.Email.Contains("@"))
                {
                    errorMessage = "El email no tiene un formato válido.";
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(cliente.Direccion) && cliente.Direccion.Length > 200)
                {
                    errorMessage = "La dirección no puede superar los 200 caracteres.";
                    return false;
                }

                errorMessage = string.Empty;
                return true;
            }
        }
    }