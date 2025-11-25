using SGHR.Domain.Base;
using SGHR.Domain.Entities.Clientes;
using SGHR.Domain.Enums;

namespace SGHR.Domain.Entities.Reservas
{
    public sealed class Reserva : BaseEntity<int>
    {

        public int IdCliente { get; set; }
        public Cliente Cliente { get; set; } = null!;
        public int IdHabitacion { get; set; }
        public Habitacion Habitacion { get; set; } = null!;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int NumeroHuespedes { get; set; }
        public decimal Total { get; set; }
        public EstadoReserva EstadoReserva { get; set; }
        public ICollection<Pago> Pagos { get; set; } = [];
        public ICollection<ReservaServicio> ReservaServicios { get; set; } = [];
    }

}