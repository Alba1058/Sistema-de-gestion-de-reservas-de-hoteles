using SGHR.Application.DTOs.Reservas.ServicioAdicional;
using SGHR.Domain.Base;

namespace SGHR.Application.Interfaces.Reservas
{
    public interface IServicioAdicionalService : IBaseService<CreateServicioAdicionalDTO, UpdateServicioAdicionalDTO, DeleteServicioAdicionalDTO, ServicioAdicionalDTO>
    {
        Task<OperationResult<List<ServicioAdicionalDTO>>> GetServiciosDisponiblesAsync();
    }
}