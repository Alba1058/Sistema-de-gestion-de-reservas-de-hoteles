using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Clientes.Cliente;
using SGHR.Application.Interfaces.Clientes;
using SGHR.Application.Mappers;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Clientes;
using SGHR.Persistence.Interfaces.Clientes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGHR.Application.Services.Clientes
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly ILogger<ClienteService> _logger;

        public ClienteService(IClienteRepository clienteRepository, ILogger<ClienteService> logger)
        {
            _clienteRepository = clienteRepository ?? throw new ArgumentNullException(nameof(clienteRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ClienteDTO?> GetByIdAsync(int id)
        {
            if (id <= 0) return null;

            try
            {
                var cliente = await _clienteRepository.GetEntityByIdAsync(id);
                if (cliente == null || cliente.IsDeleted) return null;
                return ConfigurationMappers.ToClienteDto(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetByIdAsync para Cliente id {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<ClienteDTO>> GetAllAsync()
        {
            try
            {
                var clientes = await _clienteRepository.GetAllAsync();
                return clientes?
                    .Where(c => !c.IsDeleted)
                    .Select(ConfigurationMappers.ToClienteDto)
                    .ToList() ?? new List<ClienteDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetAllAsync para Clientes");
                throw;
            }
        }

        public async Task<IEnumerable<ClienteDTO>> GetAllWithReservasAsync()
        {
            try
            {
                var clientes = await _clienteRepository.GetClientesConReservasAsync();
                return clientes?
                    .Where(c => !c.IsDeleted)
                    .Select(ConfigurationMappers.ToClienteDto)
                    .ToList() ?? new List<ClienteDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetAllWithReservasAsync para Clientes");
                throw;
            }
        }

        public async Task<ClienteDTO> CreateAsync(ClienteCreateDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            // DTO validaciones
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new ArgumentException("Nombre es requerido", nameof(dto.Nombre));
            if (string.IsNullOrWhiteSpace(dto.DocumentoDeIdentidad))
                throw new ArgumentException("DocumentoDeIdentidad es requerido", nameof(dto.DocumentoDeIdentidad));

            var entity = ConfigurationMappers.CreateClienteEntity(dto);

            try
            {
                var result = await _clienteRepository.SaveEntityAsync(entity);
                if (!result.Success)
                {
                    _logger.LogWarning("SaveEntityAsync fallo: {Message}", result.Message);
                    throw new InvalidOperationException(result.Message ?? "fallo al guardar cliente");
                }

                return ConfigurationMappers.ToClienteDto(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando Cliente");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(ClienteUpdateDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Id <= 0) return false;

            try
            {
                var cliente = await _clienteRepository.GetEntityByIdAsync(dto.Id);
                if (cliente == null || cliente.IsDeleted) return false;

                ConfigurationMappers.UpdateClienteFromDto(cliente, dto);
                var result = await _clienteRepository.UpdateEntityAsync(cliente);
                if (!result.Success)
                {
                    _logger.LogWarning("UpdateEntityAsync fallo para Cliente {Id}: {Message}", dto.Id, result.Message);
                }
                return result.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando Cliente id {Id}", dto.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(ClienteDeleteDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Id <= 0) return false;

            try
            {
                var cliente = await _clienteRepository.GetEntityByIdAsync(dto.Id);
                if (cliente == null || cliente.IsDeleted) return false;

                OperationResult result;
                try
                {
                    result = await _clienteRepository.DeleteEntityAsync(cliente);
                }
                catch (MissingMethodException)
                {
                    cliente.IsDeleted = true;
                    cliente.FechaModificacion = DateTime.UtcNow;
                    result = await _clienteRepository.UpdateEntityAsync(cliente);
                }

                if (!result.Success)
                    _logger.LogWarning("fallo al borrar Cliente {Id}: {Message}", dto.Id, result.Message);

                return result.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error borrando Cliente id {Id}", dto.Id);
                throw;
            }
        }
    }
}