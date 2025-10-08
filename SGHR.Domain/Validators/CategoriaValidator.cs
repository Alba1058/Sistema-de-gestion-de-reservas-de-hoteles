using SGHR.Domain.Entities.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Domain.Validators
{
    public class CategoriaValidator
    {
        public bool Validate(Categoria categoria, out string errorMessage)
        {
            if (!ValidationHelper.NotNull(categoria, "Categoría", out errorMessage))
                return false;

            if (!ValidationHelper.Required(categoria.Nombre, "Nombre", out errorMessage))
                return false;

            if (!ValidationHelper.MaxLength(categoria.Descripcion, 200, "Descripción", out errorMessage))
                return false;

            return true;
        }
    }
}