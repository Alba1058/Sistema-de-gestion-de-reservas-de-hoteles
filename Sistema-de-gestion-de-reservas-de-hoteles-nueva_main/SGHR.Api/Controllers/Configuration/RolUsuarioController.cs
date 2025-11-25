using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Configuration.RolUsuario;
using SGHR.Application.Interfaces.Configuration;

namespace SGHR.Api.Controllers.Configuration
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolUsuarioController : ControllerBase
    {
        private readonly IRolUsuarioService _service;
        private readonly ILogger<RolUsuarioController> _logger;

        public RolUsuarioController(IRolUsuarioService service, ILogger<RolUsuarioController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var res = await _service.GetAllAsync();
            return res?.Success == true ? Ok(res) : BadRequest(res);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _service.GetByIdAsync(id);
            return res?.Success == true ? Ok(res) : BadRequest(res);
        }

        [HttpGet("activos")]
        public async Task<IActionResult> GetActivos()
        {
            var res = await _service.GetRolesActivosAsync();
            return res?.Success == true ? Ok(res) : BadRequest(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRolUsuarioDTO dto)
        {
            var res = await _service.CreateAsync(dto);
            return res?.Success == true ? Ok(res) : BadRequest(res);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateRolUsuarioDTO dto)
        {
            var res = await _service.UpdateAsync(dto);
            return res?.Success == true ? Ok(res) : BadRequest(res);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var dto = new DeleteRolUsuarioDTO { Id = id };
            var res = await _service.RemoveAsync(dto);
            return res?.Success == true ? Ok(res) : BadRequest(res);
        }
    }
}