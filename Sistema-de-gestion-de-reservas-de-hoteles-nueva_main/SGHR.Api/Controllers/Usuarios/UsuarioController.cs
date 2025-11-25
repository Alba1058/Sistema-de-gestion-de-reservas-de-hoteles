using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Usuarios.Usuario;
using SGHR.Application.Interfaces.Usuarios;

namespace SGHR.Api.Controllers.Usuarios
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _service;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(IUsuarioService service, ILogger<UsuarioController> logger)
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

        [HttpGet("byemail")]
        public async Task<IActionResult> GetByEmail([FromQuery] string email)
        {
            var res = await _service.GetByEmailAsync(email);
            return res?.Success == true ? Ok(res) : BadRequest(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UsuarioCreateDTO dto)
        {
            var res = await _service.CreateAsync(dto);
            return res?.Success == true ? Ok(res) : BadRequest(res);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UsuarioUpdateDTO dto)
        {
            var res = await _service.UpdateAsync(dto);
            return res?.Success == true ? Ok(res) : BadRequest(res);
        }

        [HttpDelete("{id:int}")] 
        public async Task<IActionResult> Delete(int id)
        {
            var dto = new UsuarioDeleteDTO { Id = id };

            var res = await _service.RemoveAsync(dto);
            return res?.Success == true ? Ok(res) : BadRequest(res);
        }
    }
}