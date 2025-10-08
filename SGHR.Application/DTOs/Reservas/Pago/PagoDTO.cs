using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Application.DTOs.Reservas.Pago
{
    public class PagoDTO
    {
        public int Id { get; set; }
        public int IdReserva { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public string Metodo { get; set; } = null!;
        public bool Confirmado { get; set; }
        public bool Estado { get; set; }
    }
}