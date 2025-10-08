using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Reservas.Pago;
using SGHR.Application.Interfaces.Reservas;
using SGHR.Application.Mappers;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Reservas;
using SGHR.Persistence.Interfaces.Reservas;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SGHR.Application.Services.Reservas
{
    public class PagoService : IPagoService
    {
        private readonly IPagoRepository _pagoRepository;
        private readonly ILogger<PagoService> _logger;
        private readonly IConfiguration _configuration;

        public PagoService(IPagoRepository pagoRepository,
                           ILogger<PagoService> logger,
                           IConfiguration configuration)
        {
            _pagoRepository = pagoRepository ?? throw new ArgumentNullException(nameof(pagoRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration;
        }

        public async Task<OperationResult> GetAll()
        {
            var result = new OperationResult();
            try
            {
                var pagos = await _pagoRepository.GetAllAsync();
                result.Data = pagos
                    .Where(p => !p.IsDeleted)
                    .Select(ConfigurationMappers.ToPagoDto)
                    .OrderByDescending(p => p.Id)
                    .ToList();

                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los pagos.");
                result.Success = false;
                result.Message = "Ocurrió un error al obtener los pagos.";
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
                var pago = await _pagoRepository.GetEntityByIdAsync(id);
                if (pago == null || pago.IsDeleted)
                {
                    result.Success = false;
                    result.Message = "Pago no encontrado.";
                    return result;
                }

                result.Data = ConfigurationMappers.ToPagoDto(pago);
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el pago Id={Id}", id);
                result.Success = false;
                result.Message = "Ocurrió un error al obtener el pago.";
            }
            return result;
        }

        public async Task<OperationResult> Save(CreatePagoDTO dto)
        {
            var result = new OperationResult();
            if (dto == null)
            {
                result.Success = false;
                result.Message = "Datos del pago vacíos.";
                return result;
            }

            try
            {
                var entity = ConfigurationMappers.CreatePagoEntity(dto);
                var op = await _pagoRepository.SaveEntityAsync(entity);

                if (!op.Success)
                {
                    result.Success = false;
                    result.Message = op.Message ?? "Error al guardar el pago.";
                    return result;
                }

                result.Success = true;
                result.Message = "Pago registrado correctamente.";
                result.Data = ConfigurationMappers.ToPagoDto(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar el pago.");
                result.Success = false;
                result.Message = "Ocurrió un error al guardar el pago.";
            }

            return result;
        }

        public async Task<OperationResult> Update(UpdatePagoDTO dto)
        {
            var result = new OperationResult();
            if (dto == null || dto.Id <= 0)
            {
                result.Success = false;
                result.Message = "Id de pago inválido.";
                return result;
            }

            try
            {
                var entity = await _pagoRepository.GetEntityByIdAsync(dto.Id);
                if (entity == null || entity.IsDeleted)
                {
                    result.Success = false;
                    result.Message = "Pago no encontrado.";
                    return result;
                }

                ConfigurationMappers.UpdatePagoFromDto(entity, dto);
                var op = await _pagoRepository.UpdateEntityAsync(entity);

                if (!op.Success)
                {
                    result.Success = false;
                    result.Message = op.Message ?? "Error al actualizar el pago.";
                    return result;
                }

                result.Success = true;
                result.Message = "Pago actualizado correctamente.";
                result.Data = ConfigurationMappers.ToPagoDto(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el pago Id={Id}", dto?.Id);
                result.Success = false;
                result.Message = "Ocurrió un error al actualizar el pago.";
            }

            return result;
        }

        public async Task<OperationResult> Remove(DeletePagoDTO dto)
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
                var entity = await _pagoRepository.GetEntityByIdAsync(dto.Id);
                if (entity == null || entity.IsDeleted)
                {
                    result.Success = false;
                    result.Message = "Pago no encontrado.";
                    return result;
                }

                entity.IsDeleted = true;
                entity.FechaModificacion = DateTime.UtcNow;

                var op = await _pagoRepository.UpdateEntityAsync(entity);
                if (!op.Success)
                {
                    result.Success = false;
                    result.Message = op.Message ?? "Error al eliminar el pago.";
                    return result;
                }

                result.Success = true;
                result.Message = "Pago eliminado correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el pago Id={Id}", dto?.Id);
                result.Success = false;
                result.Message = "Ocurrió un error al eliminar el pago.";
            }

            return result;
        }
    }
}
