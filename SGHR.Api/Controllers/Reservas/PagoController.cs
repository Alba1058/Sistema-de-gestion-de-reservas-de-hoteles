using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace SGHR.Api.Controllers.Reservas
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagoController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "pago1", "pago2" };
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return $"pago{id}";
        }

        [HttpPost]
        public IActionResult Post([FromBody] string value)
        {
            return Ok($"Recibido: {value}");
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] string value)
        {
            return Ok($"Actualizado {id} con {value}");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok($"Borrado {id}");
        }
    }
}