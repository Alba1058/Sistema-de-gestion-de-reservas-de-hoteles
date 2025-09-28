using SGHR.Domain.Base;
using SGHR.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Domain.Entities.Reservas
{
    public sealed class Habitacion : BaseEntity<int>
    {
        public string Numero { get; set; } = null!;
        public int IdCategoria { get; set; }    
        public int IdPiso { get; set; }       
        public EstadoHabitacion EstadoH { get; set; } 
        public decimal PrecioBase { get; set; }
        public string? Descripcion { get; set; }
    }
}