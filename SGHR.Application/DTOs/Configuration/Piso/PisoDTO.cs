using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Application.DTOs.Configuration.Piso
{
    public class PisoDTO
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public string? Descripcion { get; set; }
        public bool Estado { get; set; }
    }
}
