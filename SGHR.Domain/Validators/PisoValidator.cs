using SGHR.Domain.Entities.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Domain.Validators
{
    public class PisoValidator
    {
        public bool Validate(Piso piso, out string errorMessage)
        {
            if (!ValidationHelper.NotNull(piso, "Piso", out errorMessage))
                return false;

            if (!ValidationHelper.GreaterThanZero(piso.Numero, "Número", out errorMessage))
                return false;

            if (!ValidationHelper.MaxLength(piso.Descripcion, 100, "Descripción", out errorMessage))
                return false;

            return true;
        }
    }
}