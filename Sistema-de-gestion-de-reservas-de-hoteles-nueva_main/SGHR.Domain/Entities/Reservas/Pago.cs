using SGHR.Domain.Base;


namespace SGHR.Domain.Entities.Reservas
{
    public sealed class Pago : BaseEntity<int>
    {
        public int IdReserva { get; set; }
        public Reserva Reserva { get; set; } = null!;
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public string Metodo { get; set; } = null!;
        public bool Confirmado { get; set; } = true;
    }
}