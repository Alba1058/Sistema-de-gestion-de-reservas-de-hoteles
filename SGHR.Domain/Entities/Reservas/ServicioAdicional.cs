using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SGHR.Domain.Base;

namespace SGHR.Domain.Entities.Reservas
{
    public sealed class ServicioAdicional : BaseEntity<int>
    {
        public string Nombre { get; set; } = null!;
        public decimal Precio { get; set; }        
        public string? Descripcion { get; set; }    
    }
}