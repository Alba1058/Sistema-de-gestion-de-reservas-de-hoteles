using Microsoft.EntityFrameworkCore;
using SGHR.Domain.Validators;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Clientes;
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces.Clientes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SGHR.Persistence.Repositories.Clientes
{
    public sealed class ClienteRepository : BaseRepository<Cliente>, IClienteRepository
    {
        private readonly SGHRContext _context;
        private readonly ILogger<ClienteRepository> _logger;

        public ClienteRepository(SGHRContext context, ILogger<ClienteRepository> logger) : base(context)
        {
            _context = context;
            _logger = logger;
        }

        // Validación 
        public override async Task<OperationResult> SaveEntityAsync(Cliente entity)
        {
            var validator = new ClienteValidator();
            if (!validator.Validate(entity, out string errorMessage))
            {
                _logger.LogWarning("Fallo al guardar cliente: {Error}", errorMessage);
                return new OperationResult
                {
                    Success = false,
                    Message = errorMessage
                };
            }
            var result = await base.SaveEntityAsync(entity);

            if (result.Success)
                _logger.LogInformation("Cliente guardado correctamente: {Id}", entity.Id); 
            else
                _logger.LogError("Error al guardar cliente: {Mensaje}", result.Message);

            return result;
        }

        public override async Task<OperationResult> UpdateEntityAsync(Cliente entity)
        {
            var validator = new ClienteValidator();
            if (!validator.Validate(entity, out string errorMessage))
            {
                _logger.LogWarning("Fallo al actualizar cliente: {Error}", errorMessage); 
                return new OperationResult
                {
                    Success = false,
                    Message = errorMessage
                };
            }
            var result = await base.UpdateEntityAsync(entity);

            if (result.Success)
                _logger.LogInformation("Cliente actualizado correctamente: {Id}", entity.Id); 
            else
                _logger.LogError("Error al actualizar cliente: {Mensaje}", result.Message);

            return result;
        }
        public async Task<List<Cliente>> GetClientesConReservasAsync()
        {
            try
            {
                var clientes = await _context.Clientes
                    .Include(c => c.Reservas)
                    .Where(c => c.Reservas.Any(r => !r.IsDeleted) && !c.IsDeleted)
                    .ToListAsync();

                _logger.LogInformation("Se obtuvieron {} clientes con reservas", clientes.Count);
                return clientes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los clientes con reservas"); 
                throw;
            }
        }
    }
}