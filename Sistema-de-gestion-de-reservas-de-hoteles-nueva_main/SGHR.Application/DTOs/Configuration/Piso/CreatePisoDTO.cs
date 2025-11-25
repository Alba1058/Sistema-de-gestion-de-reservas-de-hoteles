
using System.ComponentModel.DataAnnotations;

namespace SGHR.Application.DTOs.Configuration.Piso
{
    public class CreatePisoDTO
    {
        public int Numero { get; set; }
        [MaxLength(250)]
        public string? Descripcion { get; set; }
    }
}
