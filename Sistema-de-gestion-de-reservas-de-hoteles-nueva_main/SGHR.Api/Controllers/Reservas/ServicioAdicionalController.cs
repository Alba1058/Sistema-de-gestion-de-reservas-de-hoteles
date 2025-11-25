using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Reservas.ServicioAdicional;
using SGHR.Application.Interfaces.Reservas;

namespace SGHR.Api.Controllers.Reservas
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicioAdicionalController : ControllerBase
    {
        private readonly IServicioAdicionalService _service;
        private readonly ILogger<ServicioAdicionalController> _logger;

        public ServicioAdicionalController(IServicioAdicionalService service, ILogger<ServicioAdicionalController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var res = await _service.GetAllAsync();
            return res.Success ? Ok(res) : BadRequest(res);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var res = await _service.GetByIdAsync(id);
            return res.Success ? Ok(res) : BadRequest(res);
        }

        [HttpGet("disponibles")]
        public async Task<IActionResult> GetDisponibles()
        {
            var res = await _service.GetServiciosDisponiblesAsync();
            return res.Success ? Ok(res) : BadRequest(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateServicioAdicionalDTO dto)
        {
            var res = await _service.CreateAsync(dto);
            return res.Success ? Ok(res) : BadRequest(res);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateServicioAdicionalDTO dto)
        {
            var res = await _service.UpdateAsync(dto);
            return res.Success ? Ok(res) : BadRequest(res);
        }

        [HttpDelete("{id:int}")] 
        public async Task<IActionResult> Delete(int id)
        {
            var dto = new DeleteServicioAdicionalDTO { Id = id };

            var res = await _service.RemoveAsync(dto);
            return res.Success ? Ok(res) : BadRequest(res);
        }
    }
}