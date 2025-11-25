using SGHR.Domain.Base;

namespace SGHR.Domain.Entities.Reservas
{
    public sealed class ReservaServicio : BaseEntity<int>
    {
        public int IdReserva { get; set; }
        public Reserva Reserva { get; set; } = null!;
        public int IdServicioAdicional { get; set; }
        public ServicioAdicional ServicioAdicional { get; set; } = null!;
        public int Cantidad { get; set; }
        public decimal PrecioTotal { get; set; }

        public decimal Subtotal => PrecioTotal * Cantidad;
        public ICollection<ReservaServicio> ReservaServicios { get; set; } = [];
    }
}
