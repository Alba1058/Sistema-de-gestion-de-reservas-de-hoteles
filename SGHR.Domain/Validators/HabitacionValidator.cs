using SGHR.Domain.Entities.Reservas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Domain.Validators
{
    public class HabitacionValidator
    {
        public bool Validate(Habitacion habitacion, out string errorMessage)
        {
            if (!ValidationHelper.NotNull(habitacion, "Habitación", out errorMessage))
                return false;

            if (!ValidationHelper.Required(habitacion.Numero, "Número", out errorMessage))
                return false;

            if (!ValidationHelper.GreaterThanZero(habitacion.IdCategoria, "IdCategoria", out errorMessage))
                return false;

            if (!ValidationHelper.GreaterThanZero(habitacion.IdPiso, "IdPiso", out errorMessage))
                return false;

            if (habitacion.EstadoH == 0)
            {
                errorMessage = "El estado de la habitación no es válido.";
                return false;
            }

            if (habitacion.PrecioBase <= 0)
            {
                errorMessage = "El precio base debe ser mayor a cero.";
                return false;
            }

            if (!ValidationHelper.MaxLength(habitacion.Descripcion, 200, "Descripción", out errorMessage))
                return false;

            errorMessage = string.Empty;
            return true;
        }
    }
}