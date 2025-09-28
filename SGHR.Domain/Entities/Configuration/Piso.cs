using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SGHR.Domain.Base;

namespace SGHR.Domain.Entities.Configuration
{
    public sealed class Piso : BaseEntity<int>
    {
        public int Numero { get; set; }
        public string? Descripcion { get; set; }
    }
}
