using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Reservas.Tarifa;
using SGHR.Application.Interfaces.Reservas;

namespace SGHR.Api.Controllers.Reservas
{
    [ApiController]
    [Route("api/[controller]")]
    public class TarifaController : ControllerBase
    {
        private readonly ITarifaService _service;
        private readonly ILogger<TarifaController> _logger;

        public TarifaController(ITarifaService service, ILogger<TarifaController> logger)
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTarifaDTO dto)
        {
            var res = await _service.CreateAsync(dto);
            return res.Success ? Ok(res) : BadRequest(res);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateTarifaDTO dto)
        {
            var res = await _service.UpdateAsync(dto);
            return res.Success ? Ok(res) : BadRequest(res);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var dto = new DeleteTarifaDTO { Id = id };
            var res = await _service.RemoveAsync(dto);
            return res.Success ? Ok(res) : BadRequest(res);
        }
    }
}