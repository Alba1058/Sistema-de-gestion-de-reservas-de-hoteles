using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SGHR.Domain.Base;

namespace SGHR.Domain.Entities.Clientes
{
    public sealed class Cliente : BaseEntity<int>
    {
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;    
        public string Telefono { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Direccion { get; set; }

        public ICollection<Reservas.Reserva> Reservas { get; set; } = new List<Reservas.Reserva>();
    }
}