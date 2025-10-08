using SGHR.Application.DTOs.Reservas.Reserva;
using SGHR.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Application.Interfaces.Reservas
{
    public interface IReservaService
    {
        Task<OperationResult> GetAll();
        Task<OperationResult> GetById(int id);
        Task<OperationResult> GetByClienteAsync(int idCliente);
        Task<OperationResult> GetByFechaAsync(DateTime inicio, DateTime fin);
        Task<OperationResult> Save(CreateReservaDTO dto);
        Task<OperationResult> Update(UpdateReservaDTO dto);
        Task<OperationResult> Remove(DeleteReservaDTO dto);
        Task<OperationResult> CancelarAsync(int id);
    }
}