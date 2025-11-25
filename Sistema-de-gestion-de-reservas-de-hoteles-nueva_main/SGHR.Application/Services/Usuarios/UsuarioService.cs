using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Usuarios.Usuario;
using SGHR.Application.Interfaces.Usuarios;
using SGHR.Application.Mappers.Usuarios;
using SGHR.Application.Base;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Usuarios;
using SGHR.Persistence.Interfaces.Usuarios;

namespace SGHR.Application.Services.Usuarios
{
    public sealed class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<UsuarioService> _logger;

        public UsuarioService(IUsuarioRepository usuarioRepository, ILogger<UsuarioService> logger)
        {
            _usuarioRepository = usuarioRepository;
            _logger = logger;
        }

        public async Task<OperationResult<UsuarioDTO>> CreateAsync(UsuarioCreateDTO dto)
        {
            if (!ValidationHelper.NotNull(dto, "Usuario", out var msg))
                return OperationResult<UsuarioDTO>.Fail(msg);
            if (!ValidationHelper.Required(dto.Nombre, "Nombre", out msg))
                return OperationResult<UsuarioDTO>.Fail(msg);
            if (!ValidationHelper.IsValidEmail(dto.Email, out msg))
                return OperationResult<UsuarioDTO>.Fail(msg);
            if (!ValidationHelper.Required(dto.Contrasena, "Contraseña", out msg))
                return OperationResult<UsuarioDTO>.Fail(msg);
            if (dto.Contrasena.Length < 6)
                return OperationResult<UsuarioDTO>.Fail("La contraseña debe tener al menos 6 caracteres.");

            dto.Nombre = dto.Nombre.Trim();
            dto.Email = dto.Email.Trim().ToLower();

            if (await _usuarioRepository.ExistsAsync(u => u.Email == dto.Email && !u.IsDeleted))
                return OperationResult<UsuarioDTO>.Fail("Ya existe un usuario con este correo electrónico.");

            try
            {
                var usuario = UsuarioMapper.CreateUsuarioEntity(dto);
                usuario.Contrasena = dto.Contrasena; 
                usuario.FechaCreacion = DateTime.Now;
                usuario.IsDeleted = false;

                var opResult = await _usuarioRepository.SaveEntityAsync(usuario);

                if (!opResult.Success)
                    return OperationResult<UsuarioDTO>.Fail(opResult.Message);

                return OperationResult<UsuarioDTO>.Ok(
                    UsuarioMapper.ToUsuarioDto(opResult.Data),
                    "Usuario creado correctamente."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario");
                return OperationResult<UsuarioDTO>.Fail("Ocurrió un error interno al crear el usuario.");
            }
        }

        public async Task<OperationResult<UsuarioDTO>> UpdateAsync(UsuarioUpdateDTO dto)
        {
            if (!ValidationHelper.NotNull(dto, "Usuario", out var msg))
                return OperationResult<UsuarioDTO>.Fail(msg);
            if (dto.Id <= 0)
                return OperationResult<UsuarioDTO>.Fail("El ID del usuario es inválido.");
            if (!ValidationHelper.Required(dto.Nombre, "Nombre", out msg))
                return OperationResult<UsuarioDTO>.Fail(msg);
            if (!ValidationHelper.IsValidEmail(dto.Email, out msg))
                return OperationResult<UsuarioDTO>.Fail(msg);

            try
            {
                var usuario = await _usuarioRepository.GetEntityByIdAsync(dto.Id);
                if (usuario == null)
                    return OperationResult<UsuarioDTO>.Fail("Usuario no encontrado.");

                dto.Nombre = dto.Nombre.Trim();
                dto.Email = dto.Email.Trim().ToLower();

                var allUsuarios = await _usuarioRepository.GetAllAsync();
                if (allUsuarios.Any(u => u.Email.ToLower() == dto.Email && u.Id != dto.Id && !u.IsDeleted))
                    return OperationResult<UsuarioDTO>.Fail("Ya existe otro usuario con este correo electrónico.");

                UsuarioMapper.UpdateUsuarioFromDto(usuario, dto);
                usuario.FechaModificacion = DateTime.Now;

                var opResult = await _usuarioRepository.UpdateEntityAsync(usuario);

                if (!opResult.Success)
                    return OperationResult<UsuarioDTO>.Fail(opResult.Message);

                return OperationResult<UsuarioDTO>.Ok(
                    UsuarioMapper.ToUsuarioDto(opResult.Data),
                    "Usuario actualizado correctamente."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario");
                return OperationResult<UsuarioDTO>.Fail("Ocurrió un error interno al actualizar el usuario.");
            }
        }

        public async Task<OperationResult<bool>> RemoveAsync(UsuarioDeleteDTO dto)
        {
            if (dto.Id <= 0)
                return OperationResult<bool>.Fail("El ID del usuario es inválido.");

            try
            {
                var usuario = await _usuarioRepository.GetEntityByIdAsync(dto.Id);
                if (usuario == null)
                    return OperationResult<bool>.Fail("Usuario no encontrado.");

                usuario.IsDeleted = true;
                usuario.FechaModificacion = DateTime.Now;

                var opResult = await _usuarioRepository.UpdateEntityAsync(usuario);
                if (!opResult.Success)
                    return OperationResult<bool>.Fail(opResult.Message);

                return OperationResult<bool>.Ok(true, "Usuario eliminado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar usuario");
                return OperationResult<bool>.Fail("Error interno al eliminar el usuario.");
            }
        }

        public async Task<OperationResult<UsuarioDTO>> GetByIdAsync(int id)
        {
            if (id <= 0)
                return OperationResult<UsuarioDTO>.Fail("El ID es inválido.");

            try
            {
                var usuario = await _usuarioRepository.GetEntityByIdAsync(id);
                if (usuario == null || usuario.IsDeleted)
                    return OperationResult<UsuarioDTO>.Fail("Usuario no encontrado.");

                return OperationResult<UsuarioDTO>.Ok(UsuarioMapper.ToUsuarioDto(usuario));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario por ID");
                return OperationResult<UsuarioDTO>.Fail("Error interno al consultar el usuario.");
            }
        }

        public async Task<OperationResult<List<UsuarioDTO>>> GetAllAsync()
        {
            try
            {
                var usuarios = await _usuarioRepository.GetAllAsync();
                var activos = usuarios.Where(u => !u.IsDeleted).ToList();
                var dtoList = activos.ConvertAll(UsuarioMapper.ToUsuarioDto);
                return OperationResult<List<UsuarioDTO>>.Ok(dtoList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios");
                return OperationResult<List<UsuarioDTO>>.Fail("Error interno al obtener los usuarios.");
            }
        }

        public async Task<OperationResult<UsuarioDTO>> GetByEmailAsync(string email)
        {
            if (!ValidationHelper.IsValidEmail(email, out var msg))
                return OperationResult<UsuarioDTO>.Fail(msg);

            try
            {
                var usuario = await _usuarioRepository.GetUsuarioByCorreoAsync(email);
                if (usuario == null || usuario.IsDeleted)
                    return OperationResult<UsuarioDTO>.Fail("Usuario no encontrado con el correo especificado.");

                return OperationResult<UsuarioDTO>.Ok(UsuarioMapper.ToUsuarioDto(usuario));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario por correo");
                return OperationResult<UsuarioDTO>.Fail("Error interno al consultar el usuario por correo.");
            }
        }
    }
}
