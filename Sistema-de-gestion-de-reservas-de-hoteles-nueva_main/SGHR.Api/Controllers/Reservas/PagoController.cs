using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Reservas.Pago;
using SGHR.Application.Interfaces.Reservas;

namespace SGHR.Api.Controllers.Reservas
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagoController : ControllerBase
    {
        private readonly IPagoService _service;
        private readonly ILogger<PagoController> _logger;

        public PagoController(IPagoService service, ILogger<PagoController> logger)
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
        public async Task<IActionResult> Create([FromBody] CreatePagoDTO dto)
        {
            var res = await _service.CreateAsync(dto);
            return res.Success ? Ok(res) : BadRequest(res);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdatePagoDTO dto)
        {
            var res = await _service.UpdateAsync(dto);
            return res.Success ? Ok(res) : BadRequest(res);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var dto = new DeletePagoDTO { Id = id };
            var res = await _service.RemoveAsync(dto);
            return res.Success ? Ok(res) : BadRequest(res);
        }
    }
}