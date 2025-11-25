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
    public sealed class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly ILogger<ClienteService> _logger;

        public ClienteService(IClienteRepository clienteRepository, ILogger<ClienteService> logger)
        {
            _clienteRepository = clienteRepository;
            _logger = logger;
        }

        public async Task<OperationResult<List<ClienteDTO>>> GetAllAsync()
        {
            try
            {
                var clientes = await _clienteRepository.GetAllAsync();

                var dtoList = clientes
                    .Where(c => !c.IsDeleted)
                    .Select(ClienteMapper.ToClienteDto)
                    .ToList();

                return OperationResult<List<ClienteDTO>>.Ok(dtoList, "Clientes obtenidos correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo clientes");
                return OperationResult<List<ClienteDTO>>.Fail("Error al obtener los clientes.");
            }
        }

        public async Task<OperationResult<ClienteDTO>> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return OperationResult<ClienteDTO>.Fail("El ID del cliente no es válido.");

                var entity = await _clienteRepository.GetEntityByIdAsync(id);
                if (entity == null || entity.IsDeleted) 
                    return OperationResult<ClienteDTO>.Fail("Cliente no encontrado.");

                var dto = ClienteMapper.ToClienteDto(entity);
                return OperationResult<ClienteDTO>.Ok(dto, "Cliente obtenido correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo cliente por Id");
                return OperationResult<ClienteDTO>.Fail("Error interno al obtener el cliente.");
            }
        }

        public async Task<OperationResult<ClienteDTO>> CreateAsync(ClienteCreateDTO dto)
        {
            try
            {

                if (!ValidationHelper.NotNull(dto, "Cliente", out var msg)) return OperationResult<ClienteDTO>.Fail(msg);
                if (!ValidationHelper.Required(dto.Nombre, "Nombre", out msg)) return OperationResult<ClienteDTO>.Fail(msg);
                if (!ValidationHelper.Required(dto.Apellido, "Apellido", out msg)) return OperationResult<ClienteDTO>.Fail(msg);
                if (!ValidationHelper.IsValidEmail(dto.Email, out msg)) return OperationResult<ClienteDTO>.Fail(msg);
                if (!ValidationHelper.Required(dto.Telefono, "Teléfono", out msg)) return OperationResult<ClienteDTO>.Fail(msg);
                if (!ValidationHelper.Required(dto.Identificacion, "Identificación", out msg)) return OperationResult<ClienteDTO>.Fail(msg);
                if (!ValidationHelper.MaxLength(dto.Direccion, 200, "Dirección", out msg)) return OperationResult<ClienteDTO>.Fail(msg);


                if (await _clienteRepository.ExistsAsync(x => x.Email == dto.Email && !x.IsDeleted))
                    return OperationResult<ClienteDTO>.Fail("Ya existe un cliente con este correo.");

                if (await _clienteRepository.ExistsAsync(x => x.Telefono == dto.Telefono && !x.IsDeleted))
                    return OperationResult<ClienteDTO>.Fail("Ya existe un cliente con este teléfono.");

                if (await _clienteRepository.ExistsAsync(x => x.Identificacion == dto.Identificacion && !x.IsDeleted))
                    return OperationResult<ClienteDTO>.Fail("Ya existe un cliente con esta identificación.");

                var entity = ClienteMapper.CreateClienteEntity(dto, usuario: "sistema");
                var saveResult = await _clienteRepository.SaveEntityAsync(entity);

                if (!saveResult.Success)
                    return OperationResult<ClienteDTO>.Fail(saveResult.Message);

                var createdDto = ClienteMapper.ToClienteDto(saveResult.Data!);
                return OperationResult<ClienteDTO>.Ok(createdDto, "Cliente creado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear cliente");
                return OperationResult<ClienteDTO>.Fail("Error interno al crear el cliente.");
            }
        }

        public async Task<OperationResult<ClienteDTO>> UpdateAsync(ClienteUpdateDTO dto)
        {
            try
            {
                if (dto.Id <= 0) return OperationResult<ClienteDTO>.Fail("El ID no es válido.");
                if (!ValidationHelper.NotNull(dto, "Cliente", out var msg)) return OperationResult<ClienteDTO>.Fail(msg);

                var entity = await _clienteRepository.GetEntityByIdAsync(dto.Id);
                if (entity == null) return OperationResult<ClienteDTO>.Fail("Cliente no encontrado.");

                // Validaciones
                if (!ValidationHelper.Required(dto.Nombre, "Nombre", out msg)) return OperationResult<ClienteDTO>.Fail(msg);
                if (!ValidationHelper.Required(dto.Apellido, "Apellido", out msg)) return OperationResult<ClienteDTO>.Fail(msg);
                if (!ValidationHelper.IsValidEmail(dto.Email, out msg)) return OperationResult<ClienteDTO>.Fail(msg);
                if (!ValidationHelper.Required(dto.Telefono, "Teléfono", out msg)) return OperationResult<ClienteDTO>.Fail(msg);
                if (!ValidationHelper.Required(dto.Identificacion, "Identificación", out msg)) return OperationResult<ClienteDTO>.Fail(msg);

                // Unicidad
                if (await _clienteRepository.ExistsAsync(x => x.Email == dto.Email && x.Id != dto.Id && !x.IsDeleted))
                    return OperationResult<ClienteDTO>.Fail("Otro cliente ya usa este correo.");

                if (await _clienteRepository.ExistsAsync(x => x.Telefono == dto.Telefono && x.Id != dto.Id && !x.IsDeleted))
                    return OperationResult<ClienteDTO>.Fail("Otro cliente ya usa este teléfono.");

                if (await _clienteRepository.ExistsAsync(x => x.Identificacion == dto.Identificacion && x.Id != dto.Id && !x.IsDeleted))
                    return OperationResult<ClienteDTO>.Fail("Otro cliente ya usa esta identificación.");

                ClienteMapper.UpdateClienteFromDto(entity, dto, usuario: "sistema");
                var updateOp = await _clienteRepository.UpdateEntityAsync(entity);

                if (!updateOp.Success)
                    return OperationResult<ClienteDTO>.Fail(updateOp.Message);

                var dtoResult = ClienteMapper.ToClienteDto(updateOp.Data!);
                return OperationResult<ClienteDTO>.Ok(dtoResult, "Cliente actualizado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar cliente");
                return OperationResult<ClienteDTO>.Fail("Error interno al actualizar el cliente.");
            }
        }

        public async Task<OperationResult<bool>> RemoveAsync(ClienteDeleteDTO dto)
        {
            try
            {
                if (dto.Id <= 0) return OperationResult<bool>.Fail("El ID no es válido.");

                var entity = await _clienteRepository.GetEntityByIdAsync(dto.Id);
                if (entity == null) return OperationResult<bool>.Fail("Cliente no encontrado.");

                var delOp = await _clienteRepository.DeleteEntityAsync(entity);
                if (!delOp.Success) return OperationResult<bool>.Fail(delOp.Message);

                return OperationResult<bool>.Ok(true, "Cliente eliminado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar cliente");
                return OperationResult<bool>.Fail("Error interno al eliminar el cliente.");
            }
        }

        public async Task<OperationResult<List<ClienteDTO>>> GetClientesConReservasAsync()
        {
            try
            {
                var clientes = await _clienteRepository.GetClientesConReservasAsync();
                var dtoList = clientes.Select(ClienteMapper.ToClienteDto).ToList();

                return OperationResult<List<ClienteDTO>>.Ok(dtoList, "Clientes con reservas obtenidos correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo clientes con reservas");
                return OperationResult<List<ClienteDTO>>.Fail("Error al obtener los clientes con reservas.");
            }
        }
    }
}
