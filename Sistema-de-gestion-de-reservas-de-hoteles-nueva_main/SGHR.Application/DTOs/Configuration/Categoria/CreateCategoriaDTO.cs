
using System.ComponentModel.DataAnnotations;

namespace SGHR.Application.DTOs.Configuration.Categoria
{
    public class CreateCategoriaDTO
    {
        [Required, MaxLength(100)]
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
    }
}
