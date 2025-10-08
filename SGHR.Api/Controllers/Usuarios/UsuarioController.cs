using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace SGHR.Api.Controllers.Usuarios
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "usuario1", "usuario2" };
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return $"usuario{id}";
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