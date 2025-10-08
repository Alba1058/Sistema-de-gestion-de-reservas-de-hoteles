using SGHR.Application.DTOs.Reservas.ServicioAdicional;
using SGHR.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Application.Interfaces.Reservas
{ 
public interface IServicioAdicionalService
{
    Task<OperationResult> GetAll();
    Task<OperationResult> GetById(int id);
    Task<OperationResult> GetDisponiblesAsync();
    Task<OperationResult> Save(CreateServicioAdicionalDTO dto);
    Task<OperationResult> Update(UpdateServicioAdicionalDTO dto);
    Task<OperationResult> Remove(DeleteServicioAdicionalDTO dto);
}
}