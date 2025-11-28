using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Clientes.Cliente;
using SGHR.Application.Interfaces.Clientes;
using SGHR.Application.Mappers.Clientes;
using SGHR.Application.Base;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Clientes;
using SGHR.Persistence.Interfaces.Clientes;

namespace SGHR.Application.Services.Clientes
{
    public sealed class ClienteService : BaseService, IClienteService
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteService(IClienteRepository clienteRepository, ILogger<ClienteService> logger)
            : base(logger)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<OperationResult<List<ClienteDTO>>> GetAllAsync()
        {
            return await GetAllEntitiesAsync<Cliente, ClienteDTO>(
                _clienteRepository.GetAllAsync,
                ClienteMapper.ToClienteDto,
                "Clientes");
        }

        public async Task<OperationResult<ClienteDTO>> GetByIdAsync(int id)
        {
            return await ExecuteOperationAsync<ClienteDTO>(async () =>
            {
                if (!ValidationHelper.IsValidId(id, "Cliente", out var msg))
                    return OperationResult<ClienteDTO>.Fail(msg);

                var entity = await _clienteRepository.GetEntityByIdAsync(id);
                if (!EntityValidationHelper.ValidateEntityExists(entity, "Cliente", out msg))
                    return OperationResult<ClienteDTO>.Fail(msg);

                var dto = ClienteMapper.ToClienteDto(entity);
                return OperationResult<ClienteDTO>.Ok(dto, "Cliente obtenido correctamente.");
            }, "Error interno al obtener el cliente.");
        }

        public async Task<OperationResult<ClienteDTO>> CreateAsync(ClienteCreateDTO dto)
        {
            return await ExecuteOperationAsync<ClienteDTO>(async () =>
            {
                if (!ValidationHelper.NotNull(dto, "Cliente", out var msg))
                    return OperationResult<ClienteDTO>.Fail(msg);
                if (!ValidationHelper.Required(dto.Nombre, "Nombre", out msg))
                    return OperationResult<ClienteDTO>.Fail(msg);
                if (!ValidationHelper.Required(dto.Apellido, "Apellido", out msg))
                    return OperationResult<ClienteDTO>.Fail(msg);
                if (!ValidationHelper.IsValidEmail(dto.Email, out msg))
                    return OperationResult<ClienteDTO>.Fail(msg);
                if (!ValidationHelper.Required(dto.Telefono, "Teléfono", out msg))
                    return OperationResult<ClienteDTO>.Fail(msg);
                if (!ValidationHelper.Required(dto.Identificacion, "Identificación", out msg))
                    return OperationResult<ClienteDTO>.Fail(msg);
                if (!ValidationHelper.MaxLength(dto.Direccion, 200, "Dirección", out msg))
                    return OperationResult<ClienteDTO>.Fail(msg);

                var (isValidEmail, emailMsg) = await UniquenessValidationHelper.ValidateUniquenessAsync<Cliente>(
                    _clienteRepository.ExistsAsync,
                    x => x.Email == dto.Email && !x.IsDeleted,
                    "correo electrónico",
                    "cliente");
                if (!isValidEmail)
                    return OperationResult<ClienteDTO>.Fail(emailMsg);

                var (isValidTelefono, telefonoMsg) = await UniquenessValidationHelper.ValidateUniquenessAsync<Cliente>(
                    _clienteRepository.ExistsAsync,
                    x => x.Telefono == dto.Telefono && !x.IsDeleted,
                    "teléfono",
                    "cliente");
                if (!isValidTelefono)
                    return OperationResult<ClienteDTO>.Fail(telefonoMsg);

                var (isValidIdentificacion, identificacionMsg) = await UniquenessValidationHelper.ValidateUniquenessAsync<Cliente>(
                    _clienteRepository.ExistsAsync,
                    x => x.Identificacion == dto.Identificacion && !x.IsDeleted,
                    "identificación",
                    "cliente");
                if (!isValidIdentificacion)
                    return OperationResult<ClienteDTO>.Fail(identificacionMsg);

                var entity = ClienteMapper.CreateClienteEntity(dto, usuario: "sistema");
                var saveResult = await _clienteRepository.SaveEntityAsync(entity);

                if (!saveResult.Success)
                    return OperationResult<ClienteDTO>.Fail(saveResult.Message);

                var createdDto = ClienteMapper.ToClienteDto(saveResult.Data!);
                return OperationResult<ClienteDTO>.Ok(createdDto, "Cliente creado correctamente.");
            }, "Error interno al crear el cliente.");
        }

        public async Task<OperationResult<ClienteDTO>> UpdateAsync(ClienteUpdateDTO dto)
        {
            return await ExecuteOperationAsync<ClienteDTO>(async () =>
            {
                if (!ValidationHelper.IsValidId(dto.Id, "Cliente", out var msg))
                    return OperationResult<ClienteDTO>.Fail(msg);
                if (!ValidationHelper.NotNull(dto, "Cliente", out msg))
                    return OperationResult<ClienteDTO>.Fail(msg);

                var entity = await _clienteRepository.GetEntityByIdAsync(dto.Id);
                if (!EntityValidationHelper.ValidateEntityExists(entity, "Cliente", out msg))
                    return OperationResult<ClienteDTO>.Fail(msg);

                if (!ValidationHelper.Required(dto.Nombre, "Nombre", out msg))
                    return OperationResult<ClienteDTO>.Fail(msg);
                if (!ValidationHelper.Required(dto.Apellido, "Apellido", out msg))
                    return OperationResult<ClienteDTO>.Fail(msg);
                if (!ValidationHelper.IsValidEmail(dto.Email, out msg))
                    return OperationResult<ClienteDTO>.Fail(msg);
                if (!ValidationHelper.Required(dto.Telefono, "Teléfono", out msg))
                    return OperationResult<ClienteDTO>.Fail(msg);
                if (!ValidationHelper.Required(dto.Identificacion, "Identificación", out msg))
                    return OperationResult<ClienteDTO>.Fail(msg);

                var (isValidEmail, emailMsg) = await UniquenessValidationHelper.ValidateUniquenessForUpdateAsync<Cliente>(
                    _clienteRepository.ExistsAsync,
                    x => x.Email == dto.Email && x.Id != dto.Id && !x.IsDeleted,
                    "correo electrónico",
                    "cliente");
                if (!isValidEmail)
                    return OperationResult<ClienteDTO>.Fail(emailMsg);

                var (isValidTelefono, telefonoMsg) = await UniquenessValidationHelper.ValidateUniquenessForUpdateAsync<Cliente>(
                    _clienteRepository.ExistsAsync,
                    x => x.Telefono == dto.Telefono && x.Id != dto.Id && !x.IsDeleted,
                    "teléfono",
                    "cliente");
                if (!isValidTelefono)
                    return OperationResult<ClienteDTO>.Fail(telefonoMsg);

                var (isValidIdentificacion, identificacionMsg) = await UniquenessValidationHelper.ValidateUniquenessForUpdateAsync<Cliente>(
                    _clienteRepository.ExistsAsync,
                    x => x.Identificacion == dto.Identificacion && x.Id != dto.Id && !x.IsDeleted,
                    "identificación",
                    "cliente");
                if (!isValidIdentificacion)
                    return OperationResult<ClienteDTO>.Fail(identificacionMsg);

                ClienteMapper.UpdateClienteFromDto(entity!, dto, usuario: "sistema");
                var updateOp = await _clienteRepository.UpdateEntityAsync(entity!);

                if (!updateOp.Success)
                    return OperationResult<ClienteDTO>.Fail(updateOp.Message);

                var dtoResult = ClienteMapper.ToClienteDto(updateOp.Data!);
                return OperationResult<ClienteDTO>.Ok(dtoResult, "Cliente actualizado correctamente.");
            }, "Error interno al actualizar el cliente.");
        }

        public async Task<OperationResult<bool>> RemoveAsync(ClienteDeleteDTO dto)
        {
            return await ExecuteOperationAsync(async () =>
            {
                if (!ValidationHelper.IsValidId(dto.Id, "Cliente", out var msg))
                    return OperationResult<bool>.Fail(msg);

                var entity = await _clienteRepository.GetEntityByIdAsync(dto.Id);
                if (!EntityValidationHelper.ValidateEntityExists(entity, "Cliente", out msg))
                    return OperationResult<bool>.Fail(msg);

                var delOp = await _clienteRepository.DeleteEntityAsync(entity!);
                if (!delOp.Success)
                    return OperationResult<bool>.Fail(delOp.Message);

                return OperationResult<bool>.Ok(true, "Cliente eliminado correctamente.");
            }, "Error interno al eliminar el cliente.");
        }

        public async Task<OperationResult<List<ClienteDTO>>> GetClientesConReservasAsync()
        {
            return await ExecuteOperationAsync<List<ClienteDTO>>(async () =>
            {
                var clientes = await _clienteRepository.GetClientesConReservasAsync();
                var dtoList = clientes.Select(ClienteMapper.ToClienteDto).ToList();

                return OperationResult<List<ClienteDTO>>.Ok(dtoList, "Clientes con reservas obtenidos correctamente.");
            }, "Error al obtener los clientes con reservas.");
        }
    }
}
