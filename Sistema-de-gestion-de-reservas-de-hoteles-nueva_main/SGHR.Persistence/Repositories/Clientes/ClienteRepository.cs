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

        public override async Task<OperationResult<Cliente>> SaveEntityAsync(Cliente entity)
        {
            if (entity == null)
                return OperationResult<Cliente>.Fail("El cliente no puede ser nulo.");

            if (string.IsNullOrWhiteSpace(entity.Nombre))
                return OperationResult<Cliente>.Fail("El nombre del cliente no puede estar vacío.");

            if (string.IsNullOrWhiteSpace(entity.Identificacion))
                return OperationResult<Cliente>.Fail("La identificación no puede estar vacía.");

            if (await _context.Clientes.AnyAsync(c => c.Identificacion == entity.Identificacion && !c.IsDeleted))
                return OperationResult<Cliente>.Fail("Ya existe un cliente con esa identificación.");

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
                var existing = await _context.Clientes.FindAsync(entity.Id);
                if (existing == null)
                    return OperationResult<Cliente>.Fail("Cliente no encontrado.");

                if (await _context.Clientes.AnyAsync(c => c.Identificacion == entity.Identificacion && c.Id != entity.Id && !c.IsDeleted))
                    return OperationResult<Cliente>.Fail("Ya existe otro cliente con esa identificación.");

                existing.Nombre = entity.Nombre;
                existing.Apellido = entity.Apellido;
                existing.Identificacion = entity.Identificacion;
                existing.Telefono = entity.Telefono;
                existing.Email = entity.Email;
                existing.Direccion = entity.Direccion;
                existing.UsuarioModificacion = entity.UsuarioModificacion;
                existing.FechaModificacion = DateTime.Now;

                _context.Clientes.Update(existing);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Cliente {Nombre} actualizado correctamente con ID {Id}", existing.Nombre, existing.Id);

                return OperationResult<Cliente>.Ok(existing, "Cliente actualizado correctamente.");
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
