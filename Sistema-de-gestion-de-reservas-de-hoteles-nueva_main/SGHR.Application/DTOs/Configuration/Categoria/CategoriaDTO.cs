
using System.ComponentModel.DataAnnotations;

namespace SGHR.Application.DTOs.Configuration.Categoria
{
    public class CategoriaDTO
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public bool Estado { get; set; }
    }
}
