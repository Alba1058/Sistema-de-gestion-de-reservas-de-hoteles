using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Application.DTOs.Reservas.Habitacion
{
    public class CreateHabitacionDTO
    {
        public string Numero { get; set; } = null!;
        public int IdCategoria { get; set; }
        public int IdPiso { get; set; }
        public int EstadoHabitacion { get; set; }
        public decimal PrecioBase { get; set; }
        public string? Descripcion { get; set; }
        public bool Estado { get; set; } = true;
    }
}