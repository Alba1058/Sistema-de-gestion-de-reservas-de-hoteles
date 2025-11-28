using SGHR.Application.DTOs.Reservas.Reserva;
using SGHR.Application.Interfaces.Reservas;
using SGHR.Web.ViewModels.Reservas;

namespace SGHR.Web.Infrastructure.Services.Api.Facade
{
    public class ReservaApiFacade : IReservaApiFacade
    {
        private readonly IReservaFacade _reservaFacade;
        private readonly ILogger<ReservaApiFacade> _logger;

        public ReservaApiFacade(
            IReservaFacade reservaFacade,
            ILogger<ReservaApiFacade> logger)
        {
            _reservaFacade = reservaFacade ?? throw new ArgumentNullException(nameof(reservaFacade));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CreateReservaViewModel> GetCreateReservaDataAsync()
        {
            var result = await _reservaFacade.GetCreateReservaDataAsync();
            var data = result.Data ?? new ReservaFacadeData();

            return new CreateReservaViewModel
            {
                Clientes = data.Clientes ?? new(),
                Habitaciones = data.Habitaciones ?? new(),
                Reserva = new CreateReservaDTO()
            };
        }

        public async Task<EditReservaViewModel> GetEditReservaDataAsync(int reservaId)
        {
            var result = await _reservaFacade.GetEditReservaDataAsync(reservaId);
            var data = result.Data ?? new ReservaFacadeData();
            if (data.Reserva == null || data.Reserva.Id == 0)
            {
                return new EditReservaViewModel
                {
                    Clientes = data.Clientes ?? new(),
                    Habitaciones = data.Habitaciones ?? new(),
                    Reserva = new UpdateReservaDTO { Id = 0 }
                };
            }

            return new EditReservaViewModel
            {
                Reserva = new UpdateReservaDTO
                {
                    Id = data.Reserva.Id,
                    IdCliente = data.Reserva.IdCliente,
                    IdHabitacion = data.Reserva.IdHabitacion,
                    FechaInicio = data.Reserva.FechaInicio,
                    FechaFin = data.Reserva.FechaFin,
                    NumeroHuespedes = data.Reserva.NumeroHuespedes,
                    Total = data.Reserva.Total,
                    EstadoReserva = data.Reserva.EstadoReserva,
                    Estado = data.Reserva.Estado
                },
                Clientes = data.Clientes ?? new(),
                Habitaciones = data.Habitaciones ?? new()
            };
        }
    }
}

