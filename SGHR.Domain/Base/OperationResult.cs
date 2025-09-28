using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Domain.Base
{
    public class OperationResult
    {
        public OperationResult()
        {
            this.Success = true;
        }
        public string? Message { get; set; }
        public bool Success { get; set; }
        public dynamic? Data { get; set; }
    }
}
