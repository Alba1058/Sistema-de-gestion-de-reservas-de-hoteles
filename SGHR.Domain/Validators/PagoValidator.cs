using SGHR.Domain.Entities.Reservas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Domain.Validators
{
    public class PagoValidator
    {
        public bool Validate(Pago pago, out string errorMessage)
        {
            if (!ValidationHelper.NotNull(pago, "Pago", out errorMessage))
                return false;

            if (!ValidationHelper.GreaterThanZero(pago.IdReserva, "IdReserva", out errorMessage))
                return false;

            if (!ValidationHelper.GreaterThanZero(pago.Monto, "Monto", out errorMessage))
                return false;

            if (!ValidationHelper.Required(pago.Metodo, "Método de pago", out errorMessage))
                return false;

            if (pago.FechaPago == default)
            {
                errorMessage = "La fecha de pago no es válida.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }
}