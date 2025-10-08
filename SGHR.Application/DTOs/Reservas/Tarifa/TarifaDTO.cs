using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Application.DTOs.Reservas.Tarifa
{
    public class TarifaDTO
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = null!;
        public decimal Monto { get; set; }
        public DateOnly FechaInicio { get; set; }
        public DateOnly FechaFin { get; set; }
        public decimal PrecioPorNoche { get; set; }
        public decimal Descuento { get; set; }
        public string Descripcion { get; set; } = null!;
        public int IdHabitacion { get; set; }
        public bool Estado { get; set; }
    }
}