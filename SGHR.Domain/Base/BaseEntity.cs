using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Domain.Base
{
    public abstract class BaseEntity<T>
    {
        public T Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? UsuarioModificacion { get; set; }
        public bool Estado { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

    }

}