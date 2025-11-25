using SGHR.Application.DTOs.Reservas.Habitacion;
using SGHR.Domain.Base;

namespace SGHR.Web.Infrastructure.Services.Api.Interfaces
{
    public interface IHabitacionApiService : IApiService<HabitacionDTO, CreateHabitacionDTO, UpdateHabitacionDTO, DeleteHabitacionDTO>
    {
    }
}

