using SGHR.Domain.Entities.Reservas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Domain.Validators
{
    public class TarifaValidator
    {
        public bool Validate(Tarifa tarifa, out string errorMessage)
        {
            if (!ValidationHelper.NotNull(tarifa, "Tarifa", out errorMessage))
                return false;

            if (!ValidationHelper.Required(tarifa.Tipo, "Tipo", out errorMessage))
                return false;

            if (!ValidationHelper.Required(tarifa.Descripcion, "Descripción", out errorMessage))
                return false;

            if (!ValidationHelper.GreaterThanZero(tarifa.Monto, "Monto", out errorMessage))
                return false;

            if (!ValidationHelper.GreaterThanZero(tarifa.PrecioPorNoche, "Precio por noche", out errorMessage))
                return false;

            if (!ValidationHelper.GreaterThanZero(tarifa.IdHabitacion, "IdHabitacion", out errorMessage))
                return false;

            if (tarifa.FechaInicio == default || tarifa.FechaFin == default)
            {
                errorMessage = "Las fechas de inicio y fin son obligatorias.";
                return false;
            }

            if (tarifa.FechaInicio >= tarifa.FechaFin)
            {
                errorMessage = "La fecha de inicio debe ser anterior a la fecha de fin.";
                return false;
            }

            if (tarifa.Descuento < 0 || tarifa.Descuento > 100)
            {
                errorMessage = "El descuento debe estar entre 0% y 100%.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }
}