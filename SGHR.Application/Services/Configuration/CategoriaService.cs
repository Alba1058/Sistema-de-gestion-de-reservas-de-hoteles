using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Configuration.Categoria;
using SGHR.Application.Interfaces.Configuration;
using SGHR.Application.Mappers;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Interfaces.Configuracion;

namespace SGHR.Application.Services.Configuracion
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly ILogger<CategoriaService> _logger;

        public CategoriaService(ICategoriaRepository categoriaRepository, ILogger<CategoriaService> logger)
        {
            _categoriaRepository = categoriaRepository ?? throw new ArgumentNullException(nameof(categoriaRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<CategoriaDTO>> GetAllAsync()
        {
            try
            {
                var categorias = await _categoriaRepository.GetAllAsync();
                return categorias
                    .Where(c => !c.IsDeleted)
                    .Select(ConfigurationMappers.ToCategoriaDto)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las categorías.");
                return Enumerable.Empty<CategoriaDTO>();
            }
        }

        public async Task<CategoriaDTO?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Id de categoría inválido.");
                return null;
            }

            try
            {
                var categoria = await _categoriaRepository.GetEntityByIdAsync(id);
                if (categoria == null || categoria.IsDeleted)
                    return null;

                return ConfigurationMappers.ToCategoriaDto(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categoría por Id={Id}", id);
                return null;
            }
        }

        public async Task<CategoriaDTO> CreateAsync(CreateCategoriaDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Nombre))
                throw new ArgumentException("El nombre de la categoría es requerido.");

            try
            {
                var all = await _categoriaRepository.GetAllAsync();
                if (all.Any(c => !c.IsDeleted &&
                                 string.Equals(c.Nombre, dto.Nombre, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException("Ya existe una categoría activa con ese nombre.");
                }

                var entity = ConfigurationMappers.CreateCategoriaEntity(dto);
                await _categoriaRepository.SaveEntityAsync(entity);
                return ConfigurationMappers.ToCategoriaDto(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear categoría.");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(UpdateCategoriaDTO dto)
        {
            if (dto == null || dto.Id <= 0)
                return false;

            try
            {
                var categoria = await _categoriaRepository.GetEntityByIdAsync(dto.Id);
                if (categoria == null || categoria.IsDeleted)
                    return false;

                var all = await _categoriaRepository.GetAllAsync();
                if (!string.IsNullOrWhiteSpace(dto.Nombre) &&
                    all.Any(c => c.Id != dto.Id && !c.IsDeleted &&
                                 string.Equals(c.Nombre, dto.Nombre, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException("Ya existe otra categoría activa con el mismo nombre.");
                }

                categoria.Nombre = dto.Nombre ?? categoria.Nombre;
                categoria.Descripcion = dto.Descripcion ?? categoria.Descripcion;
                categoria.FechaModificacion = DateTime.UtcNow;

                await _categoriaRepository.UpdateEntityAsync(categoria);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar categoría Id={Id}", dto.Id);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(DeleteCategoriaDTO dto)
        {
            if (dto == null || dto.Id <= 0)
                return false;

            try
            {
                var categoria = await _categoriaRepository.GetEntityByIdAsync(dto.Id);
                if (categoria == null || categoria.IsDeleted)
                    return false;

                categoria.IsDeleted = true;
                categoria.FechaModificacion = DateTime.UtcNow;
                await _categoriaRepository.UpdateEntityAsync(categoria);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar categoría Id={Id}", dto.Id);
                return false;
            }
        }

        public async Task<CategoriaDTO?> RestoreAsync(int id)
        {
            if (id <= 0)
                return null;

            try
            {
                var categoria = await _categoriaRepository.GetEntityByIdAsync(id);
                if (categoria == null || !categoria.IsDeleted)
                    return null;

                var all = await _categoriaRepository.GetAllAsync();
                if (all.Any(c => c.Id != id && !c.IsDeleted &&
                                 string.Equals(c.Nombre, categoria.Nombre, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException("No se puede restaurar: ya existe una categoría activa con el mismo nombre.");
                }

                categoria.IsDeleted = false;
                categoria.FechaModificacion = DateTime.UtcNow;
                await _categoriaRepository.UpdateEntityAsync(categoria);

                return ConfigurationMappers.ToCategoriaDto(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al restaurar categoría Id={Id}", id);
                return null;
            }
        }
    }
}
