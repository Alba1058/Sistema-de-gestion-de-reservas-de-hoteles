using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Validators;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Usuarios;
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Persistence.Repositories.Usuarios
{
    public sealed class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
    {
        private readonly SGHRContext _context;
        private readonly ILogger<UsuarioRepository> _logger;

        public UsuarioRepository(SGHRContext context, ILogger<UsuarioRepository> logger) : base(context)
        {
            _context = context;
            _logger = logger;
        }

        public async override Task<OperationResult> SaveEntityAsync(Usuario entity)
        {
            var validator = new UsuarioValidator();
            if (!validator.Validate(entity, out string errorMessage))
            {
                _logger.LogWarning("fallo al guardar usuario: {Error}", errorMessage); 
                return new OperationResult
                {
                    Success = false,
                    Message = errorMessage
                };
            }

            var result = await base.SaveEntityAsync(entity);

            if (result.Success)
                _logger.LogInformation("Usuario guardado correctamente: {Id}", entity.Id);
            else
                _logger.LogError("Error al guardar usuario: {Mensaje}", result.Message); 

            return result;
        }

        public override async Task<OperationResult> UpdateEntityAsync(Usuario entity)
        {
            var validator = new UsuarioValidator();
            if (!validator.Validate(entity, out string errorMessage))
            {
                _logger.LogWarning("fallo al actualizar usuario: {Error}", errorMessage);
                return new OperationResult
                {
                    Success = false,
                    Message = errorMessage
                };
            }

            var result = await base.UpdateEntityAsync(entity);

            if (result.Success)
                _logger.LogInformation("Usuario actualizado correctamente: {Id}", entity.Id); 
            else
                _logger.LogError("Error al actualizar usuario: {Mensaje}", result.Message);

            return result;
        }

        public async Task<Usuario?> GetUsuarioByCorreoAsync(string correo)
        {
            try
            {
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == correo && !u.IsDeleted);

                if (usuario != null)
                    _logger.LogInformation("Usuario obtenido por correo: {Correo}", correo); 
                else
                    _logger.LogWarning("No se encontró usuario con el correo: {Correo}", correo); 

                return usuario;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario por correo {Correo}", correo); 
                throw;
            }
        }
    }
}
