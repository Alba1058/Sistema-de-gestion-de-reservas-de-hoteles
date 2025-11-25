using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Reservas.Reserva;
using SGHR.Application.Interfaces.Reservas;

namespace SGHR.Api.Controllers.Reservas
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservaController : ControllerBase
    {
        private readonly IReservaService _service;
        private readonly ILogger<ReservaController> _logger;

        public ReservaController(IReservaService service, ILogger<ReservaController> logger)
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

        [HttpGet("byfecha")]
        public async Task<IActionResult> GetByFecha([FromQuery] DateTime inicio, [FromQuery] DateTime fin)
        {
            var res = await _service.GetReservasPorFechaAsync(inicio, fin);
            return res.Success ? Ok(res) : BadRequest(res);
        }

        [HttpGet("bycliente/{clienteId:int}")]
        public async Task<IActionResult> GetByCliente(int clienteId)
        {
            var res = await _service.GetReservasPorClienteAsync(clienteId);
            return res.Success ? Ok(res) : BadRequest(res);
        }

        [HttpPost("cancel/{id:int}")]
        public async Task<IActionResult> Cancelar(int id)
        {
            var res = await _service.CancelarReservaAsync(id);
            return res.Success ? Ok(res) : BadRequest(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReservaDTO dto)
        {
            var res = await _service.CreateAsync(dto);
            return res.Success ? Ok(res) : BadRequest(res);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateReservaDTO dto)
        {
            var res = await _service.UpdateAsync(dto);
            return res.Success ? Ok(res) : BadRequest(res);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var dto = new DeleteReservaDTO { Id = id };
            var res = await _service.RemoveAsync(dto);
            return res.Success ? Ok(res) : BadRequest(res);
        }
    }
}