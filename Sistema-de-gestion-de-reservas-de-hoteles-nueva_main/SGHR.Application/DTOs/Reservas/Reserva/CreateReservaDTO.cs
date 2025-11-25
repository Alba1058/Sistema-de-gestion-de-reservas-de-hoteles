
namespace SGHR.Application.DTOs.Reservas.Reserva
{
    public class CreateReservaDTO
    {
        public int IdCliente { get; set; }
        public int IdHabitacion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int NumeroHuespedes { get; set; }
        public decimal Total { get; set; }
        public int EstadoReserva { get; set; }
    }
}