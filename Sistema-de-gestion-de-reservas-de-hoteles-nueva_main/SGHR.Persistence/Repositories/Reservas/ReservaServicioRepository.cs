using Microsoft.Extensions.Logging;
using SGHR.Domain.Entities.Reservas;
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces.Reservas;

namespace SGHR.Persistence.Repositories.Reservas
{
    public sealed class ReservaServicioRepository : BaseRepository<ReservaServicio>, IReservaServicioRepository
    {

        private readonly ILogger<ReservaServicioRepository> _logger;
        public ReservaServicioRepository(SGHRContext context, ILogger<ReservaServicioRepository> logger) : base(context)
        {
            _logger = logger;
        }
    }
}
