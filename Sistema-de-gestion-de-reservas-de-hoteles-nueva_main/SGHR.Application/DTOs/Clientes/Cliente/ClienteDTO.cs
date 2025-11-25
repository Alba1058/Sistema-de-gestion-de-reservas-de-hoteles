
namespace SGHR.Application.DTOs.Clientes.Cliente
{
    public class ClienteDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public string? Direccion { get; set; }
        public string Identificacion { get; set; } = null!;
    }
}
