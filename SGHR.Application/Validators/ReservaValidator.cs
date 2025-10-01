using SGHR.Domain.Entities.Reservas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Application.Validators
{
    public class ReservaValidator
    {
        public bool Validate(Reserva reserva, out string errorMessage)
        {
            if (!ValidationHelper.NotNull(reserva, "Reserva", out errorMessage))
                return false;

            if (!ValidationHelper.GreaterThanZero(reserva.IdCliente, "IdCliente", out errorMessage))
                return false;

            if (!ValidationHelper.GreaterThanZero(reserva.IdHabitacion, "IdHabitacion", out errorMessage))
                return false;

            if (reserva.FechaInicio == default || reserva.FechaFin == default)
            {
                errorMessage = "Las fechas de la reserva son obligatorias.";
                return false;
            }

            if (reserva.FechaInicio >= reserva.FechaFin)
            {
                errorMessage = "La fecha de inicio debe ser anterior a la fecha de fin.";
                return false;
            }

            if (reserva.NumeroHuespedes <= 0)
            {
                errorMessage = "El número de huéspedes debe ser mayor a cero.";
                return false;
            }

            if (reserva.Total <= 0)
            {
                errorMessage = "El total de la reserva debe ser mayor a cero.";
                return false;
            }

            if (reserva.EstadoReserva == 0) 
            {
                errorMessage = "El estado de la reserva no es válido.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }
}