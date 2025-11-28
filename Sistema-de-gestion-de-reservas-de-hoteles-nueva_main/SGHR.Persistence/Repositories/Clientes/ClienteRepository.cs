using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Clientes;
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces.Clientes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGHR.Persistence.Repositories.Clientes
{
    public sealed class ClienteRepository : BaseRepository<Cliente>, IClienteRepository
    {
        private readonly ILogger<ClienteRepository> _logger;

        public ClienteRepository(SGHRContext context, ILogger<ClienteRepository> logger)
            : base(context)
        {
            _logger = logger;
        }

        public override async Task<List<Cliente>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("ClienteRepository.GetAllAsync: Iniciando consulta a la base de datos");
                var clientes = await _entities.ToListAsync();
                _logger.LogInformation("ClienteRepository.GetAllAsync: Se obtuvieron {Count} clientes de la base de datos", clientes.Count);
                return clientes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ClienteRepository.GetAllAsync: Error al obtener clientes de la base de datos: {Message}", ex.Message);
                throw;
            }
        }

        public override async Task<OperationResult<Cliente>> SaveEntityAsync(Cliente entity)
        {
            try
            {
                var result = await base.SaveEntityAsync(entity);

                if (result.Success)
                    _logger.LogInformation("Cliente {Nombre} guardado correctamente con ID {Id}", entity.Nombre, entity.Id);
                else
                    _logger.LogWarning("Error al guardar cliente: {Mensaje}", result.Message);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al guardar cliente");
                return OperationResult<Cliente>.Fail("Error interno al guardar cliente.");
            }
        }

        public override async Task<OperationResult<Cliente>> UpdateEntityAsync(Cliente entity)
        {
            try
            {
                var result = await base.UpdateEntityAsync(entity);

                if (result.Success)
                    _logger.LogInformation("Cliente {Nombre} actualizado correctamente con ID {Id}", entity.Nombre, entity.Id);
                else
                    _logger.LogWarning("Error al actualizar cliente: {Mensaje}", result.Message);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar cliente");
                return OperationResult<Cliente>.Fail("Error interno al actualizar cliente.");
            }
        }
        public async Task<Cliente?> GetClienteByIdentificacionAsync(string identificacion)
        {
            try
            {
                var cliente = await _context.Clientes
                    .FirstOrDefaultAsync(c => c.Identificacion == identificacion && !c.IsDeleted);

                if (cliente != null)
                    _logger.LogInformation("Cliente obtenido por identificación {Identificacion}", identificacion);
                else
                    _logger.LogWarning("No se encontró cliente con la identificación {Identificacion}", identificacion);

                return cliente;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cliente por identificación {Identificacion}", identificacion);
                return null;
            }
        }

        public async Task<List<Cliente>> GetClientesConReservasAsync()
        {
            try
            {
                var clientes = await _context.Clientes
                    .Include(c => c.Reservas) 
                    .Where(c => c.Reservas.Any() && !c.IsDeleted)
                    .ToListAsync();

                _logger.LogInformation("Clientes con reservas recuperados correctamente ({Count})", clientes.Count);

                return clientes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener clientes con reservas");
                return new List<Cliente>();
            }
        }
    }
}
