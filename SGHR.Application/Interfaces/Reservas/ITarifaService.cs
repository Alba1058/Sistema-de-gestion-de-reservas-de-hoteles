using SGHR.Application.DTOs.Reservas.Tarifa;
using SGHR.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Application.Interfaces.Reservas
{
    public interface ITarifaService
    {
        Task<OperationResult> GetAll();
        Task<OperationResult> GetById(int id);
        Task<OperationResult> Save(CreateTarifaDTO dto);
        Task<OperationResult> Update(UpdateTarifaDTO dto);
        Task<OperationResult> Remove(DeleteTarifaDTO dto);
    }
}