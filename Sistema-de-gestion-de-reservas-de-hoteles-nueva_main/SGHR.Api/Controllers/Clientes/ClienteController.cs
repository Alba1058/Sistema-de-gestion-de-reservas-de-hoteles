using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Clientes.Cliente;
using SGHR.Application.Interfaces.Clientes;

namespace SGHR.Api.Controllers.Clientes
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _service;
        private readonly ILogger<ClienteController> _logger;

        public ClienteController(IClienteService service, ILogger<ClienteController> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

            return res?.Success == true ? Ok(res) : NotFound(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClienteCreateDTO dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("ClienteController.Create: DTO recibido es null");
                return BadRequest(new { message = "El objeto ClienteCreateDTO es requerido" });
            }

            _logger.LogInformation("ClienteController.Create: Intentando crear cliente - Nombre: {Nombre}, Email: {Email}", dto.Nombre, dto.Email);

            var res = await _service.CreateAsync(dto);
            
            if (res == null)
            {
                _logger.LogError("ClienteController.Create: Respuesta del servicio es null");
                return StatusCode(500, new { message = "Respuesta nula del servicio." });
            }

            if (!res.Success)
            {
                _logger.LogWarning("ClienteController.Create: Error al crear cliente - {Message}", res.Message);
                return BadRequest(res);
            }

            _logger.LogInformation("ClienteController.Create: Cliente creado exitosamente - ID: {Id}", res.Data?.Id);
            
            try
            {
                var idProp = res.Data?.GetType().GetProperty("Id")?.GetValue(res.Data);
                if (idProp != null)
                    return CreatedAtAction(nameof(GetById), new { id = idProp }, res);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "ClienteController.Create: Error al obtener ID del cliente creado");
            }

            return Created(string.Empty, res);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ClienteUpdateDTO dto)
        {
            var res = await _service.UpdateAsync(dto);
            return res?.Success == true ? Ok(res) : BadRequest(res);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var dto = new ClienteDeleteDTO { Id = id };

            var res = await _service.RemoveAsync(dto);
            return res?.Success == true ? Ok(res) : BadRequest(res);
        }

        [HttpGet("con-reservas")]
        public async Task<IActionResult> GetClientesConReservas()
        {
            var res = await _service.GetClientesConReservasAsync();
            return res?.Success == true ? Ok(res) : BadRequest(res);
        }
    }
}