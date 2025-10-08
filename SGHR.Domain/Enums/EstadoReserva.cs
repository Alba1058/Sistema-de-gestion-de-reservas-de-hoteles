using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Domain.Enums
{
    public enum EstadoReserva
    {
        Activa = 1,
        Pendiente = 2,
        Confirmada = 3,
        Cancelada = 4,
        Finalizada = 5,
        Ausente = 6 //en caso de que el cliente no se presente
    }
}
