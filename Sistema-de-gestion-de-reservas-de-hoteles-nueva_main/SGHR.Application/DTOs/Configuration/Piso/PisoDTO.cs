
using System.ComponentModel.DataAnnotations;

namespace SGHR.Application.DTOs.Configuration.Piso
{
    public class PisoDTO
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        [MaxLength(250)]
        public string? Descripcion { get; set; }
        public bool Estado { get; set; }
    }
}
