using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Configuration.Categoria;
using SGHR.Domain.Base;
using SGHR.Web.Infrastructure.Services.Api.Base;
using SGHR.Web.Infrastructure.Services.Api.Interfaces;

namespace SGHR.Web.Infrastructure.Services.Api
{
    public class CategoriaApiService : BaseApiService<CategoriaDTO, CreateCategoriaDTO, UpdateCategoriaDTO, DeleteCategoriaDTO>, ICategoriaApiService
    {
        public CategoriaApiService(HttpClient httpClient, ILogger<CategoriaApiService> logger)
            : base(httpClient, logger)
        {
        }

        protected override string EntityName => "Categoría";
        protected override string BaseEndpoint => "Categoria";
    }
}

