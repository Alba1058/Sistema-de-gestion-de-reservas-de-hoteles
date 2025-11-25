using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Configuration.Piso;
using SGHR.Application.Interfaces.Configuration;

namespace SGHR.Api.Controllers.Configuration
{
    [ApiController]
    [Route("api/[controller]")]
    public class PisoController : ControllerBase
    {
        private readonly IPisoService _service;
        private readonly ILogger<PisoController> _logger;

        public PisoController(IPisoService service, ILogger<PisoController> logger)
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePisoDTO dto)
        {
            var res = await _service.CreateAsync(dto);
            return res?.Success == true ? Ok(res) : BadRequest(res);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdatePisoDTO dto)
        {
            var res = await _service.UpdateAsync(dto);
            return res?.Success == true ? Ok(res) : BadRequest(res);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var dto = new DeletePisoDTO { Id = id };
            var res = await _service.RemoveAsync(dto);
            return res?.Success == true ? Ok(res) : BadRequest(res);
        }
    }
}