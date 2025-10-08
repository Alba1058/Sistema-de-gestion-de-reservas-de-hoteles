using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Application.DTOs.Clientes.Cliente
{
    public class ClienteCreateDTO
    {
        public string Nombre { get; set; } = null!;
        public string DocumentoDeIdentidad { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Telefono { get; set; } = null!;
    }
}
