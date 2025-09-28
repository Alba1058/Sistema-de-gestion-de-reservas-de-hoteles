using SGHR.Domain.Base;
using SGHR.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Domain.Entities.Reservas
{
    public sealed class Reserva : BaseEntity<int>
    {
        public Reserva() {
           EstadoReserva = EstadoReserva.Activa;
        }
        public int IdCliente { get; set; }
        public int IdHabitacion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int NumeroHuespedes { get; set; }
        public decimal Total { get; set; }
        public EstadoReserva EstadoReserva { get; set; }
    }

}