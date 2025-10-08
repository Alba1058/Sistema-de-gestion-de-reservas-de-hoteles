using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace SGHR.Api.Controllers.Reservas
{
    [Route("api/[controller]")]
    [ApiController]
    public class TarifaController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "tarifa1", "tarifa2" };
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return $"tarifa{id}";
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