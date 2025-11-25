using SGHR.Domain.Base;
using SGHR.Domain.Entities.Reservas;

namespace SGHR.Domain.Entities.Clientes
{
    public sealed class Cliente : BaseEntity<int>
    {

        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Direccion { get; set; }

        public string Identificacion { get; set; } = null!;

        public List<Reserva> Reservas { get; set; } = [];

    }
}