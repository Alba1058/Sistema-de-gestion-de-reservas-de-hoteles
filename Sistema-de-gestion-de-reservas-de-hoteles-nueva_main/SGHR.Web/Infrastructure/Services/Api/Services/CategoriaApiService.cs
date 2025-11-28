using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Configuration.Categoria;
using SGHR.Domain.Base;
using SGHR.Web.Infrastructure.Services.Api.Base;
using SGHR.Web.Infrastructure.Services.Api.Interfaces;

namespace SGHR.Web.Infrastructure.Services.Api.Services
{
    public class CategoriaApiService : BaseApiService<CategoriaDTO, CreateCategoriaDTO, UpdateCategoriaDTO, DeleteCategoriaDTO>, ICategoriaApiService
    {
        public CategoriaApiService(IHttpClientFactory httpClientFactory, ILogger<CategoriaApiService> logger)
            : base(httpClientFactory, logger)
        {
        }

        protected override string EntityName => "Categoría";
        protected override string BaseEndpoint => "Categoria";
    }
}

