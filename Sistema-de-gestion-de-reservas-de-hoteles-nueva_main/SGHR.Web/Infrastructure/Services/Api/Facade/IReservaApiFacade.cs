using SGHR.Web.ViewModels.Reservas;

namespace SGHR.Web.Infrastructure.Services.Api.Facade
{
    public interface IReservaApiFacade
    {
        Task<CreateReservaViewModel> GetCreateReservaDataAsync();
        Task<EditReservaViewModel> GetEditReservaDataAsync(int reservaId);
    }
}

