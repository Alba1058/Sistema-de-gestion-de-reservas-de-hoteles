using Microsoft.Extensions.Logging;
using SGHR.Application.Base;
using SGHR.Application.DTOs.Usuarios.Usuario;
using SGHR.Application.Interfaces.Authentication;
using SGHR.Application.Mappers.Usuarios;
using SGHR.Domain.Base;
using SGHR.Persistence.Interfaces.Usuarios;

namespace SGHR.Application.Services.Authentication
{
    public class AuthenticationServices : BaseService, IAuthenticationServices
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public AuthenticationServices(
            ILogger<AuthenticationServices> logger,
            IUsuarioRepository usuarioRepository) : base(logger)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<OperationResult<UsuarioDTO>> LoginSesionAsync(string email, string password)
        {
            return await ExecuteOperationAsync<UsuarioDTO>(async () =>
            {
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                    return OperationResult<UsuarioDTO>.Fail("Debe proporcionar el correo y la contraseña.");

                if (!ValidationHelper.IsValidEmail(email, out var msg))
                    return OperationResult<UsuarioDTO>.Fail(msg);

                var usuario = await _usuarioRepository.GetUsuarioByCorreoAsync(email);

                if (!EntityValidationHelper.ValidateEntityExists(usuario, "Usuario", out msg))
                    return OperationResult<UsuarioDTO>.Fail(msg);

                if (usuario.Contrasena != password)
                    return OperationResult<UsuarioDTO>.Fail("Contraseña incorrecta.");

                var usuarioDto = UsuarioMapper.ToUsuarioDto(usuario);

                return OperationResult<UsuarioDTO>.Ok(
                    usuarioDto,
                    $"Usuario {usuario.Nombre} inició sesión correctamente."
                );
            }, "Error interno al procesar la solicitud.");
        }
    }
}
