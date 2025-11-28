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
    public sealed class UsuarioService : BaseService, IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository, ILogger<UsuarioService> logger)
            : base(logger)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<OperationResult<UsuarioDTO>> CreateAsync(UsuarioCreateDTO dto)
        {
            return await ExecuteOperationAsync<UsuarioDTO>(async () =>
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

                var (isValidEmail, emailMsg) = await UniquenessValidationHelper.ValidateUniquenessAsync<Usuario>(
                    _usuarioRepository.ExistsAsync,
                    u => u.Email == dto.Email && !u.IsDeleted,
                    "correo electrónico",
                    "usuario");
                if (!isValidEmail)
                    return OperationResult<UsuarioDTO>.Fail(emailMsg);

                var usuario = UsuarioMapper.CreateUsuarioEntity(dto);
                usuario.Contrasena = dto.Contrasena;

                var opResult = await _usuarioRepository.SaveEntityAsync(usuario);

                if (!opResult.Success)
                    return OperationResult<UsuarioDTO>.Fail(opResult.Message);

                return OperationResult<UsuarioDTO>.Ok(
                    UsuarioMapper.ToUsuarioDto(opResult.Data),
                    "Usuario creado correctamente."
                );
            }, "Ocurrió un error interno al crear el usuario.");
        }

        public async Task<OperationResult<UsuarioDTO>> UpdateAsync(UsuarioUpdateDTO dto)
        {
            return await ExecuteOperationAsync<UsuarioDTO>(async () =>
            {
                if (!ValidationHelper.NotNull(dto, "Usuario", out var msg))
                    return OperationResult<UsuarioDTO>.Fail(msg);
                if (!ValidationHelper.IsValidId(dto.Id, "Usuario", out msg))
                    return OperationResult<UsuarioDTO>.Fail(msg);
                if (!ValidationHelper.Required(dto.Nombre, "Nombre", out msg))
                    return OperationResult<UsuarioDTO>.Fail(msg);
                if (!ValidationHelper.IsValidEmail(dto.Email, out msg))
                    return OperationResult<UsuarioDTO>.Fail(msg);

                var usuario = await _usuarioRepository.GetEntityByIdAsync(dto.Id);
                if (!EntityValidationHelper.ValidateEntityExists(usuario, "Usuario", out msg))
                    return OperationResult<UsuarioDTO>.Fail(msg);

                dto.Nombre = dto.Nombre.Trim();
                dto.Email = dto.Email.Trim().ToLower();

                var (isValidEmail, emailMsg) = await UniquenessValidationHelper.ValidateUniquenessForUpdateAsync<Usuario>(
                    _usuarioRepository.ExistsAsync,
                    u => u.Email == dto.Email && u.Id != dto.Id && !u.IsDeleted,
                    "correo electrónico",
                    "usuario");
                if (!isValidEmail)
                    return OperationResult<UsuarioDTO>.Fail(emailMsg);

                UsuarioMapper.UpdateUsuarioFromDto(usuario, dto);

                var opResult = await _usuarioRepository.UpdateEntityAsync(usuario);

                if (!opResult.Success)
                    return OperationResult<UsuarioDTO>.Fail(opResult.Message);

                return OperationResult<UsuarioDTO>.Ok(
                    UsuarioMapper.ToUsuarioDto(opResult.Data),
                    "Usuario actualizado correctamente."
                );
            }, "Ocurrió un error interno al actualizar el usuario.");
        }

        public async Task<OperationResult<bool>> RemoveAsync(UsuarioDeleteDTO dto)
        {
            return await ExecuteOperationAsync(async () =>
            {
                if (!ValidationHelper.IsValidId(dto.Id, "Usuario", out var msg))
                    return OperationResult<bool>.Fail(msg);

                var usuario = await _usuarioRepository.GetEntityByIdAsync(dto.Id);
                if (!EntityValidationHelper.ValidateEntityExists(usuario, "Usuario", out msg))
                    return OperationResult<bool>.Fail(msg);

                var opResult = await _usuarioRepository.DeleteEntityAsync(usuario);
                if (!opResult.Success)
                    return OperationResult<bool>.Fail(opResult.Message);

                return OperationResult<bool>.Ok(true, "Usuario eliminado correctamente.");
            }, "Error interno al eliminar el usuario.");
        }

        public async Task<OperationResult<UsuarioDTO>> GetByIdAsync(int id)
        {
            return await ExecuteOperationAsync<UsuarioDTO>(async () =>
            {
                if (!ValidationHelper.IsValidId(id, "Usuario", out var msg))
                    return OperationResult<UsuarioDTO>.Fail(msg);

                var usuario = await _usuarioRepository.GetEntityByIdAsync(id);
                if (!EntityValidationHelper.ValidateEntityExists(usuario, "Usuario", out msg))
                    return OperationResult<UsuarioDTO>.Fail(msg);

                return OperationResult<UsuarioDTO>.Ok(UsuarioMapper.ToUsuarioDto(usuario));
            }, "Error interno al consultar el usuario.");
        }

        public async Task<OperationResult<List<UsuarioDTO>>> GetAllAsync()
        {
            return await GetAllEntitiesAsync<Usuario, UsuarioDTO>(
                _usuarioRepository.GetAllAsync,
                UsuarioMapper.ToUsuarioDto,
                "Usuarios");
        }

        public async Task<OperationResult<UsuarioDTO>> GetByEmailAsync(string email)
        {
            return await ExecuteOperationAsync<UsuarioDTO>(async () =>
            {
                if (!ValidationHelper.IsValidEmail(email, out var msg))
                    return OperationResult<UsuarioDTO>.Fail(msg);

                var usuario = await _usuarioRepository.GetUsuarioByCorreoAsync(email);
                if (!EntityValidationHelper.ValidateEntityExists(usuario, "Usuario", out msg))
                    return OperationResult<UsuarioDTO>.Fail(msg);

                return OperationResult<UsuarioDTO>.Ok(UsuarioMapper.ToUsuarioDto(usuario));
            }, "Error interno al consultar el usuario por correo.");
        }
    }
}
