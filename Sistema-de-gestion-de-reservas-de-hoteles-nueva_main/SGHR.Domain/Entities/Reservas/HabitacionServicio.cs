using SGHR.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Domain.Entities.Reservas
{
    public sealed class HabitacionServicio : BaseEntity<int> 
    {
        public int IdHabitacion { get; set; }
        public Habitacion Habitacion { get; set; } = null!;

        public int IdServicioAdicional { get; set; }
        public ServicioAdicional ServicioAdicional { get; set; } = null!;
    }
}
