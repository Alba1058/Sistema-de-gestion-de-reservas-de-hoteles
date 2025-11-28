using SGHR.Application.DTOs.Clientes.Cliente;
using SGHR.Application.DTOs.Reservas.Habitacion;
using SGHR.Application.DTOs.Reservas.Reserva;

namespace SGHR.Web.ViewModels.Reservas
{
    public class EditReservaViewModel
    {
        public UpdateReservaDTO Reserva { get; set; } = new();
        public List<ClienteDTO> Clientes { get; set; } = new();
        public List<HabitacionDTO> Habitaciones { get; set; } = new();
        public Dictionary<int, string> EstadosReserva { get; set; } = new()
        {
            { 1, "Activa" },
            { 2, "Pendiente" },
            { 3, "Confirmada" },
            { 4, "Cancelada" },
            { 5, "Finalizada" },
            { 6, "Ausente" }
        };
    }
}

