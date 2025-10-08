using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Reservas.Reserva;
using SGHR.Application.Interfaces.Reservas;
using SGHR.Application.Mappers;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Reservas;
using SGHR.Persistence.Interfaces.Reservas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGHR.Application.Services.Reservas
{
    public class ReservaService : IReservaService
    {
        private readonly IReservaRepository _reservaRepository;
        private readonly ILogger<ReservaService> _logger;

        public ReservaService(IReservaRepository reservaRepository, ILogger<ReservaService> logger)
        {
            _reservaRepository = reservaRepository ?? throw new ArgumentNullException(nameof(reservaRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult> GetAll()
        {
            var result = new OperationResult();
            try
            {
                var reservas = await _reservaRepository.GetAllAsync();
                result.Data = reservas
                    .Where(r => !r.IsDeleted)
                    .Select(ConfigurationMappers.ToReservaDto)
                    .ToList();
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reservas.");
                result.Success = false;
                result.Message = "Ocurrió un error al obtener las reservas.";
            }
            return result;
        }

        public async Task<OperationResult> GetById(int id)
        {
            var result = new OperationResult();
            if (id <= 0)
            {
                result.Success = false;
                result.Message = "Id inválido.";
                return result;
            }

            try
            {
                var reserva = await _reservaRepository.GetEntityByIdAsync(id);
                if (reserva == null || reserva.IsDeleted)
                {
                    result.Success = false;
                    result.Message = "Reserva no encontrada.";
                    return result;
                }

                result.Success = true;
                result.Data = ConfigurationMappers.ToReservaDto(reserva);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reserva Id={Id}", id);
                result.Success = false;
                result.Message = "Ocurrió un error al obtener la reserva.";
            }
            return result;
        }

        public async Task<OperationResult> Save(CreateReservaDTO dto)
        {
            var result = new OperationResult();
            if (dto == null)
            {
                result.Success = false;
                result.Message = "Datos vacíos.";
                return result;
            }

            try
            {
                var entity = ConfigurationMappers.CreateReservaEntity(dto);
                var op = await _reservaRepository.SaveEntityAsync(entity);
                if (!op.Success)
                {
                    result.Success = false;
                    result.Message = op.Message ?? "Error al guardar la reserva.";
                    return result;
                }

                result.Success = true;
                result.Data = ConfigurationMappers.ToReservaDto(entity);
                result.Message = "Reserva creada correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar reserva.");
                result.Success = false;
                result.Message = "Ocurrió un error al guardar la reserva.";
            }
            return result;
        }

        public async Task<OperationResult> Update(UpdateReservaDTO dto)
        {
            var result = new OperationResult();
            if (dto == null || dto.Id <= 0)
            {
                result.Success = false;
                result.Message = "Id inválido.";
                return result;
            }

            try
            {
                var reserva = await _reservaRepository.GetEntityByIdAsync(dto.Id);
                if (reserva == null || reserva.IsDeleted)
                {
                    result.Success = false;
                    result.Message = "Reserva no encontrada.";
                    return result;
                }

                ConfigurationMappers.UpdateReservaFromDto(reserva, dto);

                var op = await _reservaRepository.UpdateEntityAsync(reserva);
                if (!op.Success)
                {
                    result.Success = false;
                    result.Message = op.Message ?? "Error al actualizar la reserva.";
                    return result;
                }

                result.Success = true;
                result.Message = "Reserva actualizada correctamente.";
                result.Data = ConfigurationMappers.ToReservaDto(reserva);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar reserva Id={Id}", dto?.Id);
                result.Success = false;
                result.Message = "Ocurrió un error al actualizar la reserva.";
            }
            return result;
        }

        public async Task<OperationResult> Remove(DeleteReservaDTO dto)
        {
            var result = new OperationResult();
            if (dto == null || dto.Id <= 0)
            {
                result.Success = false;
                result.Message = "Id inválido.";
                return result;
            }

            try
            {
                var reserva = await _reservaRepository.GetEntityByIdAsync(dto.Id);
                if (reserva == null || reserva.IsDeleted)
                {
                    result.Success = false;
                    result.Message = "Reserva no encontrada.";
                    return result;
                }

                reserva.IsDeleted = true;
                reserva.FechaModificacion = DateTime.UtcNow;

                var op = await _reservaRepository.UpdateEntityAsync(reserva);
                if (!op.Success)
                {
                    result.Success = false;
                    result.Message = op.Message ?? "Error al eliminar la reserva.";
                    return result;
                }

                result.Success = true;
                result.Message = "Reserva eliminada correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar reserva Id={Id}", dto?.Id);
                result.Success = false;
                result.Message = "Ocurrió un error al eliminar la reserva.";
            }
            return result;
        }

        public async Task<OperationResult> CancelarAsync(int id)
        {
            var result = new OperationResult();
            if (id <= 0)
            {
                result.Success = false;
                result.Message = "Id inválido.";
                return result;
            }

            try
            {
                var reserva = await _reservaRepository.GetEntityByIdAsync(id);
                if (reserva == null || reserva.IsDeleted)
                {
                    result.Success = false;
                    result.Message = "Reserva no encontrada.";
                    return result;
                }

                reserva.EstadoReserva = Domain.Enums.EstadoReserva.Cancelada;
                reserva.FechaModificacion = DateTime.UtcNow;

                var op = await _reservaRepository.UpdateEntityAsync(reserva);
                if (!op.Success)
                {
                    result.Success = false;
                    result.Message = op.Message ?? "Error al cancelar la reserva.";
                    return result;
                }

                result.Success = true;
                result.Message = "Reserva cancelada correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar reserva Id={Id}", id);
                result.Success = false;
                result.Message = "Ocurrió un error al cancelar la reserva.";
            }
            return result;
        }

        public async Task<OperationResult> GetByClienteAsync(int idCliente)
        {
            var result = new OperationResult();
            try
            {
                var reservas = await _reservaRepository.GetReservasPorClienteAsync(idCliente);
                result.Data = reservas
                    .Where(r => !r.IsDeleted)
                    .Select(ConfigurationMappers.ToReservaDto)
                    .ToList();
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reservas por cliente Id={IdCliente}", idCliente);
                result.Success = false;
                result.Message = "Ocurrió un error al obtener las reservas del cliente.";
            }
            return result;
        }

        public async Task<OperationResult> GetByFechaAsync(DateTime inicio, DateTime fin)
        {
            var result = new OperationResult();
            try
            {
                var reservas = await _reservaRepository.GetReservasPorFechaAsync(inicio, fin);
                result.Data = reservas
                    .Where(r => !r.IsDeleted)
                    .Select(ConfigurationMappers.ToReservaDto)
                    .ToList();
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reservas entre fechas {Inicio} - {Fin}", inicio, fin);
                result.Success = false;
                result.Message = "Ocurrió un error al obtener las reservas por rango de fechas.";
            }
            return result;
        }
    }
}