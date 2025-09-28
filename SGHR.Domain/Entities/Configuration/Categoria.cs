using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SGHR.Domain.Base;

namespace SGHR.Domain.Entities.Configuration
{
    public sealed class Categoria : BaseEntity<int>
    {
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
    }
}
