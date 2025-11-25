
namespace SGHR.Domain.Base
{
    public abstract class BaseEntity<T>
    {
        public T Id { get; set; } = default!;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaModificacion { get; set; }
        public string? UsuarioModificacion { get; set; }
        public bool Estado { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public string? UsuarioCreacion { get; set; }
        public string? UsuarioEliminacion { get; set; }
        public DateTime? FechaEliminacion { get; set; }
    }
}