using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Application.DTOs.Reservas.Pago
{
    public class CreatePagoDTO
    {
        public int IdReserva { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public string Metodo { get; set; } = null!;
        public bool Confirmado { get; set; } = true;
        public bool Estado { get; set; } = true;
    }
}