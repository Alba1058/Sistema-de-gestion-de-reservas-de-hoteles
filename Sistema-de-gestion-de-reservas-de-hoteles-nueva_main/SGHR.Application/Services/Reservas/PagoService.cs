using Microsoft.Extensions.Logging;
using SGHR.Application.Base;
using SGHR.Application.DTOs.Reservas.Pago;
using SGHR.Application.Interfaces.Reservas;
using SGHR.Application.Mappers.Reservas;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Reservas;
using SGHR.Persistence.Interfaces.Reservas;

namespace SGHR.Application.Services.Reservas
{
    public class PagoService : BaseService, IPagoService
    {
        private readonly IPagoRepository _pagoRepository;

        public PagoService(IPagoRepository pagoRepository, ILogger<PagoService> logger)
            : base(logger)
        {
            _pagoRepository = pagoRepository;
        }

        public async Task<OperationResult<PagoDTO>> CreateAsync(CreatePagoDTO dto)
        {
            return await ExecuteOperationAsync<PagoDTO>(async () =>
            {
                if (!ValidationHelper.NotNull(dto, nameof(CreatePagoDTO), out var msg))
                    return OperationResult<PagoDTO>.Fail(msg);

                if (!ValidationHelper.Required(dto.Metodo, "Método de pago", out msg))
                    return OperationResult<PagoDTO>.Fail(msg);

                if (dto.Monto <= 0)
                    return OperationResult<PagoDTO>.Fail("El monto debe ser mayor que 0.");

                var entity = PagoMapper.CreatePagoEntity(dto);

                var saveOp = await _pagoRepository.SaveEntityAsync(entity);
                if (!saveOp.Success)
                    return OperationResult<PagoDTO>.Fail(saveOp.Message);

                var dtoResult = PagoMapper.ToPagoDto(saveOp.Data!);
                return OperationResult<PagoDTO>.Ok(dtoResult, "Pago creado correctamente.");

            }, "Error interno al crear el pago.");
        }

        public async Task<OperationResult<PagoDTO>> UpdateAsync(UpdatePagoDTO dto)
        {
            return await ExecuteOperationAsync<PagoDTO>(async () =>
            {
                if (!ValidationHelper.NotNull(dto, nameof(UpdatePagoDTO), out var msg))
                    return OperationResult<PagoDTO>.Fail(msg);

                var entity = await _pagoRepository.GetEntityByIdAsync(dto.Id);
                if (entity == null)
                    return OperationResult<PagoDTO>.Fail("Pago no encontrado.");

                if (dto.Monto <= 0)
                    return OperationResult<PagoDTO>.Fail("El monto debe ser mayor que 0.");

                PagoMapper.UpdatePagoFromDto(entity, dto);

                var updateOp = await _pagoRepository.UpdateEntityAsync(entity);
                if (!updateOp.Success)
                    return OperationResult<PagoDTO>.Fail(updateOp.Message);

                var dtoResult = PagoMapper.ToPagoDto(updateOp.Data!);
                return OperationResult<PagoDTO>.Ok(dtoResult, "Pago actualizado correctamente.");

            }, "Error interno al actualizar el pago.");
        }

        public async Task<OperationResult<bool>> RemoveAsync(DeletePagoDTO dto)
        {
            return await ExecuteOperationAsync(async () =>
            {
                if (!ValidationHelper.NotNull(dto, nameof(DeletePagoDTO), out var msg))
                    return OperationResult<bool>.Fail(msg);

                var entity = await _pagoRepository.GetEntityByIdAsync(dto.Id);
                if (entity == null)
                    return OperationResult<bool>.Fail("Pago no encontrado.");

                var delOp = await _pagoRepository.DeleteEntityAsync(entity);
                return delOp;

            }, "Error interno al eliminar el pago.");
        }

        public async Task<OperationResult<List<PagoDTO>>> GetAllAsync()
        {
            return await GetAllEntitiesAsync<Pago, PagoDTO>(
                _pagoRepository.GetAllAsync,
                PagoMapper.ToPagoDto,
                "Pagos");
        }

        public async Task<OperationResult<PagoDTO>> GetByIdAsync(int id)
        {
            return await ExecuteOperationAsync<PagoDTO>(async () =>
            {
                var entity = await _pagoRepository.GetEntityByIdAsync(id);
                if (entity == null)
                    return OperationResult<PagoDTO>.Fail("Pago no encontrado.");

                var dto = PagoMapper.ToPagoDto(entity);
                return OperationResult<PagoDTO>.Ok(dto, "Pago obtenido correctamente.");

            }, "Error interno al obtener el pago por ID.");
        }
    }
}
