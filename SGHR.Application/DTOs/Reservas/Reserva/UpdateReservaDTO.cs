using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Application.DTOs.Reservas.Reserva
{
    public class UpdateReservaDTO
    {
        public int Id { get; set; }
        public int IdCliente { get; set; }
        public int IdHabitacion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int NumeroHuespedes { get; set; }
        public decimal Total { get; set; }
        public int EstadoReserva { get; set; }
        public bool Estado { get; set; }
    }
}