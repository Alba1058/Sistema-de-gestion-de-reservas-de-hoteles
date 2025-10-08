using SGHR.Application.DTOs.Configuration.Piso;
using SGHR.Application.DTOs.Reservas.Pago;
using SGHR.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Application.Interfaces.Reservas
{
    public interface IPagoService
    {
        Task<OperationResult> GetAll();
        Task<OperationResult> GetById(int id);
        Task<OperationResult> Save(CreatePagoDTO dto);
        Task<OperationResult> Update(UpdatePagoDTO dto);
        Task<OperationResult> Remove(DeletePagoDTO dto);
    }
}