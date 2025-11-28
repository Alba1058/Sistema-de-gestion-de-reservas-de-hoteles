using Microsoft.Extensions.Logging;
using SGHR.Application.Base;
using SGHR.Application.DTOs.Reservas.Reserva;
using SGHR.Application.Interfaces.Reservas;
using SGHR.Application.Mappers.Reservas;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Reservas;
using SGHR.Persistence.Interfaces.Clientes;
using SGHR.Persistence.Interfaces.Reservas;

namespace SGHR.Application.Services.Reservas
{
    public sealed class ReservaService : BaseService, IReservaService
    {
        private readonly IReservaRepository _reservaRepository;
        private readonly IReservaServicioRepository _reservaServicioRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IHabitacionRepository _habitacionRepository;

        public ReservaService(
            IReservaRepository reservaRepository,
            IReservaServicioRepository reservaServicioRepository,
            IClienteRepository clienteRepository,
            IHabitacionRepository habitacionRepository,
            ILogger<ReservaService> logger) : base(logger)
        {
            _reservaRepository = reservaRepository;
            _reservaServicioRepository = reservaServicioRepository;
            _clienteRepository = clienteRepository;
            _habitacionRepository = habitacionRepository;
        }

        public async Task<OperationResult<ReservaDTO>> CreateAsync(CreateReservaDTO dto)
        {
            return await ExecuteOperationAsync<ReservaDTO>(async () =>
            {
                if (!ValidationHelper.NotNull(dto, "Reserva", out var msg))
                    return OperationResult<ReservaDTO>.Fail(msg);

                if (dto.IdCliente <= 0)
                    return OperationResult<ReservaDTO>.Fail("El cliente es obligatorio.");
                if (dto.IdHabitacion <= 0)
                    return OperationResult<ReservaDTO>.Fail("La habitación es obligatoria.");
                if (dto.FechaInicio >= dto.FechaFin)
                    return OperationResult<ReservaDTO>.Fail("La fecha de inicio debe ser anterior a la fecha de fin.");
                if (dto.Total <= 0)
                    return OperationResult<ReservaDTO>.Fail("El total debe ser mayor que cero.");
                if (dto.NumeroHuespedes <= 0)
                    return OperationResult<ReservaDTO>.Fail("Debe indicar el número de huéspedes.");

                var cliente = await _clienteRepository.GetEntityByIdAsync(dto.IdCliente);
                if (!EntityValidationHelper.ValidateRelatedEntity(cliente, "cliente", out msg))
                    return OperationResult<ReservaDTO>.Fail(msg);

                var habitacion = await _habitacionRepository.GetEntityByIdAsync(dto.IdHabitacion);
                if (!EntityValidationHelper.ValidateRelatedEntity(habitacion, "habitación", out msg))
                    return OperationResult<ReservaDTO>.Fail(msg);

                var reservasOp = await _reservaRepository.GetReservasPorFechaAsync(dto.FechaInicio, dto.FechaFin);
                if (reservasOp.Success && reservasOp.Data != null &&
                    reservasOp.Data.Any(r => r.IdHabitacion == dto.IdHabitacion &&
                                             r.EstadoReserva == Domain.Enums.EstadoReserva.Activa))
                {
                    return OperationResult<ReservaDTO>.Fail("La habitación ya está reservada en esas fechas.");
                }

                var entity = ReservaMapper.CreateReservaEntity(dto);

                var saveOp = await _reservaRepository.SaveEntityAsync(entity);
                if (!saveOp.Success)
                    return OperationResult<ReservaDTO>.Fail(saveOp.Message);

                return OperationResult<ReservaDTO>.Ok(
                    ReservaMapper.ToReservaDto(saveOp.Data!), "Reserva creada correctamente.");
            }, "Error interno al crear reserva.");
        }

        public async Task<OperationResult<ReservaDTO>> UpdateAsync(UpdateReservaDTO dto)
        {
            return await ExecuteOperationAsync<ReservaDTO>(async () =>
            {
                if (!ValidationHelper.NotNull(dto, "Reserva", out var msg))
                    return OperationResult<ReservaDTO>.Fail(msg);
                if (dto.Id <= 0)
                    return OperationResult<ReservaDTO>.Fail("El ID de la reserva es inválido.");
                if (dto.FechaInicio >= dto.FechaFin)
                    return OperationResult<ReservaDTO>.Fail("La fecha de inicio debe ser anterior a la fecha de fin.");

                var entity = await _reservaRepository.GetEntityByIdAsync(dto.Id);
                if (!EntityValidationHelper.ValidateEntityExists(entity, "Reserva", out msg))
                    return OperationResult<ReservaDTO>.Fail(msg);

                if (dto.IdCliente > 0)
                {
                    var cliente = await _clienteRepository.GetEntityByIdAsync(dto.IdCliente);
                    if (!EntityValidationHelper.ValidateRelatedEntity(cliente, "cliente", out msg))
                        return OperationResult<ReservaDTO>.Fail(msg);
                }

                if (dto.IdHabitacion > 0)
                {
                    var habitacion = await _habitacionRepository.GetEntityByIdAsync(dto.IdHabitacion);
                    if (!EntityValidationHelper.ValidateRelatedEntity(habitacion, "habitación", out msg))
                        return OperationResult<ReservaDTO>.Fail(msg);
                }

                var reservasOp = await _reservaRepository.GetReservasPorFechaAsync(dto.FechaInicio, dto.FechaFin);
                if (reservasOp.Success && reservasOp.Data != null &&
                    reservasOp.Data.Any(r => r.IdHabitacion == dto.IdHabitacion &&
                                             r.Id != dto.Id &&
                                             r.EstadoReserva == Domain.Enums.EstadoReserva.Activa))
                {
                    return OperationResult<ReservaDTO>.Fail("La habitación ya está reservada en esas fechas.");
                }

                ReservaMapper.UpdateReservaFromDto(entity, dto);

                var upOp = await _reservaRepository.UpdateEntityAsync(entity);
                if (!upOp.Success)
                    return OperationResult<ReservaDTO>.Fail(upOp.Message);

                return OperationResult<ReservaDTO>.Ok(
                    ReservaMapper.ToReservaDto(upOp.Data!), "Reserva actualizada correctamente.");
            }, "Error interno al actualizar reserva.");
        }

        public async Task<OperationResult<bool>> RemoveAsync(DeleteReservaDTO dto)
        {
            return await ExecuteOperationAsync<bool>(async () =>
            {
                if (dto.Id <= 0)
                    return OperationResult<bool>.Fail("El ID de la reserva es inválido.");

                var entity = await _reservaRepository.GetEntityByIdAsync(dto.Id);
                if (!EntityValidationHelper.ValidateEntityExists(entity, "Reserva", out var msg))
                    return OperationResult<bool>.Fail(msg);

                var delOp = await _reservaRepository.DeleteEntityAsync(entity);
                if (!delOp.Success)
                    return OperationResult<bool>.Fail(delOp.Message);

                return OperationResult<bool>.Ok(true, "Reserva eliminada correctamente.");
            }, "Error interno al eliminar reserva.");
        }

        public async Task<OperationResult<ReservaDTO>> GetByIdAsync(int id)
        {
            return await ExecuteOperationAsync<ReservaDTO>(async () =>
            {
                if (id <= 0)
                    return OperationResult<ReservaDTO>.Fail("El ID es inválido.");

                var entity = await _reservaRepository.GetEntityByIdAsync(id);
                if (!EntityValidationHelper.ValidateEntityExists(entity, "Reserva", out var msg))
                    return OperationResult<ReservaDTO>.Fail(msg);

                return OperationResult<ReservaDTO>.Ok(ReservaMapper.ToReservaDto(entity));
            }, "Error interno al obtener reserva por ID.");
        }

        public async Task<OperationResult<List<ReservaDTO>>> GetAllAsync()
        {
            return await GetAllEntitiesAsync<Reserva, ReservaDTO>(
                _reservaRepository.GetAllAsync,
                ReservaMapper.ToReservaDto,
                "Reservas");
        }

        public async Task<OperationResult<List<ReservaDTO>>> GetReservasPorFechaAsync(DateTime inicio, DateTime fin)
        {
            return await ExecuteOperationAsync<List<ReservaDTO>>(async () =>
            {
                if (inicio >= fin)
                    return OperationResult<List<ReservaDTO>>.Fail("El rango de fechas es inválido.");

                var listOp = await _reservaRepository.GetReservasPorFechaAsync(inicio, fin);
                if (!listOp.Success)
                    return OperationResult<List<ReservaDTO>>.Fail(listOp.Message);

                var dtos = listOp.Data?.Select(ReservaMapper.ToReservaDto).ToList() ?? new List<ReservaDTO>();
                return OperationResult<List<ReservaDTO>>.Ok(dtos, "Reservas obtenidas por fecha.");
            }, "Error interno al obtener reservas por fecha.");
        }

        public async Task<OperationResult<List<ReservaDTO>>> GetReservasPorClienteAsync(int clienteId)
        {
            return await ExecuteOperationAsync<List<ReservaDTO>>(async () =>
            {
                if (clienteId <= 0)
                    return OperationResult<List<ReservaDTO>>.Fail("El ID del cliente es inválido.");

                var listOp = await _reservaRepository.GetReservasPorClienteAsync(clienteId);
                if (!listOp.Success)
                    return OperationResult<List<ReservaDTO>>.Fail(listOp.Message);

                var dtos = listOp.Data?.Select(ReservaMapper.ToReservaDto).ToList() ?? new List<ReservaDTO>();
                return OperationResult<List<ReservaDTO>>.Ok(dtos, "Reservas del cliente obtenidas correctamente.");
            }, "Error interno al obtener reservas por cliente.");
        }

        public async Task<OperationResult<bool>> CancelarReservaAsync(int reservaId)
        {
            return await ExecuteOperationAsync<bool>(async () =>
            {
                if (reservaId <= 0)
                    return OperationResult<bool>.Fail("El ID de la reserva es inválido.");

                var reserva = await _reservaRepository.GetEntityByIdAsync(reservaId);
                if (!EntityValidationHelper.ValidateEntityExists(reserva, "Reserva", out var msg))
                    return OperationResult<bool>.Fail(msg);

                var deleteOp = await _reservaRepository.DeleteEntityAsync(reserva);
                if (!deleteOp.Success)
                    return OperationResult<bool>.Fail(deleteOp.Message);

                return OperationResult<bool>.Ok(true, "Reserva cancelada correctamente.");
            }, "Error interno al cancelar reserva.");
        }
    }
}
