using SGHR.Domain.Entities.Reservas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Application.Validators
{
    public class ServicioAdicionalValidator
    {
        public bool Validate(ServicioAdicional servicio, out string errorMessage)
        {
            if (!ValidationHelper.NotNull(servicio, "Servicio adicional", out errorMessage))
                return false;

            if (!ValidationHelper.Required(servicio.Nombre, "Nombre", out errorMessage))
                return false;

            if (!ValidationHelper.GreaterThanZero(servicio.Precio, "Precio", out errorMessage))
                return false;

            if (!ValidationHelper.MaxLength(servicio.Descripcion, 255, "Descripción", out errorMessage))
                return false;

            errorMessage = string.Empty;
            return true;
        }
    }
}