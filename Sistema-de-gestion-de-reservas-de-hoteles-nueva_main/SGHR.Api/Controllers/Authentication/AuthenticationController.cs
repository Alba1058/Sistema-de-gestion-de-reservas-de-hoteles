using Microsoft.AspNetCore.Mvc;
using SGHR.Application.Interfaces.Authentication;
using SGHR.Application.DTOs.Authentication;

namespace SGHR.Api.Controllers.Authentication
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationServices _authenticationServices;

        public AuthenticationController(IAuthenticationServices authenticationServices)
        {
            _authenticationServices = authenticationServices;
        }

        // POST: api/Authentication/login
        [HttpPost("login")]
        public async Task<IActionResult> LoginAuthenticationAsync([FromBody] AuthenticationDTO dto)
        {
            var res = await _authenticationServices.LoginSesionAsync(dto.Correo, dto.Password);

            if (res.Success)
                return Ok(res);

            return BadRequest(res.Message);
        }
    }
}
