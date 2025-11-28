using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Clientes.Cliente;
using SGHR.Application.DTOs.Reservas.Habitacion;
using SGHR.Application.DTOs.Reservas.Reserva;
using SGHR.Application.Interfaces.Clientes;
using SGHR.Application.Interfaces.Reservas;
using SGHR.Domain.Base;

namespace SGHR.Application.Facades_Classes.Reservas
{
    public class ReservaFacade : IReservaFacade
    {
        private readonly IClienteService _clienteService;
        private readonly IHabitacionService _habitacionService;
        private readonly IReservaService _reservaService;
        private readonly ILogger<ReservaFacade> _logger;

        public ReservaFacade(
            IClienteService clienteService,
            IHabitacionService habitacionService,
            IReservaService reservaService,
            ILogger<ReservaFacade> logger)
        {
            _clienteService = clienteService ?? throw new ArgumentNullException(nameof(clienteService));
            _habitacionService = habitacionService ?? throw new ArgumentNullException(nameof(habitacionService));
            _reservaService = reservaService ?? throw new ArgumentNullException(nameof(reservaService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<ReservaFacadeData>> GetCreateReservaDataAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo datos para crear reserva");

                var clientesResult = await _clienteService.GetAllAsync();
                var habitacionesResult = await _habitacionService.GetAllAsync();
                if (!clientesResult.Success)
                {
                    _logger.LogError("Error al obtener clientes para crear reserva: {Message}. Success: {Success}, Data: {Data}", 
                        clientesResult.Message, clientesResult.Success, clientesResult.Data?.Count ?? 0);
                }
                else
                {
                    _logger.LogInformation("Clientes obtenidos exitosamente para crear reserva: {Count} clientes", 
                        clientesResult.Data?.Count ?? 0);
                }

                if (!habitacionesResult.Success)
                {
                    _logger.LogError("Error al obtener habitaciones para crear reserva: {Message}. Success: {Success}, Data: {Data}", 
                        habitacionesResult.Message, habitacionesResult.Success, habitacionesResult.Data?.Count ?? 0);
                }
                else
                {
                    _logger.LogInformation("Habitaciones obtenidas exitosamente para crear reserva: {Count} habitaciones", 
                        habitacionesResult.Data?.Count ?? 0);
                }

                var clientesList = clientesResult.Data ?? new();
                var habitacionesList = habitacionesResult.Data ?? new();

                return OperationResult<ReservaFacadeData>.Ok(new ReservaFacadeData
                {
                    Clientes = clientesList,
                    Habitaciones = habitacionesList
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener datos para crear reserva: {Message}. StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return OperationResult<ReservaFacadeData>.Ok(new ReservaFacadeData
                {
                    Clientes = new(),
                    Habitaciones = new()
                }, $"Error al obtener datos: {ex.Message}");
            }
        }

        public async Task<OperationResult<ReservaFacadeData>> GetEditReservaDataAsync(int reservaId)
        {
            try
            {
                _logger.LogInformation("Obteniendo datos para editar reserva con ID {ReservaId}", reservaId);

                var reservaResult = await _reservaService.GetByIdAsync(reservaId);
                var clientesResult = await _clienteService.GetAllAsync();
                var habitacionesResult = await _habitacionService.GetAllAsync();
                if (!reservaResult.Success || reservaResult.Data == null)
                {
                    _logger.LogWarning("Reserva con ID {ReservaId} no encontrada", reservaId);
                    return OperationResult<ReservaFacadeData>.Ok(new ReservaFacadeData
                    {
                        Reserva = null,
                        Clientes = clientesResult.Data ?? new(),
                        Habitaciones = habitacionesResult.Data ?? new()
                    }, "Reserva no encontrada");
                }

                if (!clientesResult.Success)
                {
                    _logger.LogError("Error al obtener clientes para editar reserva ID {ReservaId}: {Message}. Success: {Success}, Data: {Data}", 
                        reservaId, clientesResult.Message, clientesResult.Success, clientesResult.Data?.Count ?? 0);
                }
                else
                {
                    _logger.LogInformation("Clientes obtenidos exitosamente para editar reserva ID {ReservaId}: {Count} clientes", 
                        reservaId, clientesResult.Data?.Count ?? 0);
                }

                if (!habitacionesResult.Success)
                {
                    _logger.LogError("Error al obtener habitaciones para editar reserva ID {ReservaId}: {Message}. Success: {Success}, Data: {Data}", 
                        reservaId, habitacionesResult.Message, habitacionesResult.Success, habitacionesResult.Data?.Count ?? 0);
                }
                else
                {
                    _logger.LogInformation("Habitaciones obtenidas exitosamente para editar reserva ID {ReservaId}: {Count} habitaciones", 
                        reservaId, habitacionesResult.Data?.Count ?? 0);
                }

                var clientesList = clientesResult.Data ?? new();
                var habitacionesList = habitacionesResult.Data ?? new();

                _logger.LogInformation("ReservaFacadeData creado - Reserva ID: {ReservaId}, Clientes: {ClientesCount}, Habitaciones: {HabitacionesCount}", 
                    reservaResult.Data.Id, clientesList.Count, habitacionesList.Count);

                return OperationResult<ReservaFacadeData>.Ok(new ReservaFacadeData
                {
                    Reserva = reservaResult.Data,
                    Clientes = clientesList,
                    Habitaciones = habitacionesList
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener datos para editar reserva con ID {ReservaId}: {Message}. StackTrace: {StackTrace}", reservaId, ex.Message, ex.StackTrace);
                return OperationResult<ReservaFacadeData>.Ok(new ReservaFacadeData
                {
                    Reserva = null,
                    Clientes = new(),
                    Habitaciones = new()
                }, $"Error al obtener datos: {ex.Message}");
            }
        }
    }
}

