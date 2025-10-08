using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Validators;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Helpers;
using SGHR.Persistence.Interfaces.Configuracion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Persistence.Repositories.Configuration
{
    public class CategoriaRepositoryADO : ICategoriaRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CategoriaRepositoryADO> _logger;
        private readonly string _connectionString;

        public CategoriaRepositoryADO(IConfiguration configuration, ILogger<CategoriaRepositoryADO> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _connectionString = _configuration.GetConnectionString("SghrConnString");
        }

        public async Task<OperationResult> SaveEntityAsync(Categoria entity)
        {
            var validator = new CategoriaValidator();
            if (!validator.Validate(entity, out string errorMessage))
                return new OperationResult { Success = false, Message = errorMessage };

            var result = new OperationResult();

            try
            {
                var query = @"
                    INSERT INTO Categoria
                    (Nombre, Descripcion, Estado, IsDeleted, FechaCreacion, UsuarioCreacion)
                    VALUES (@Nombre, @Descripcion, @Estado, 0, GETDATE(), @UsuarioCreacion)";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Nombre", entity.Nombre),
                    new SqlParameter("@Descripcion", entity.Descripcion ?? string.Empty),
                    new SqlParameter("@Estado", entity.Estado),
                    new SqlParameter("@UsuarioCreacion", (object?)entity.UsuarioCreacion ?? DBNull.Value)
                };

                int rows = await SqlHelper.ExecuteNonQueryAsync(_connectionString, query, parameters);

                result.Success = rows > 0;
                result.Message = rows > 0 ? "Categoría guardada correctamente." : "No se pudo guardar la categoría.";
                result.Data = entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la categoría");
                result.Success = false;
                result.Message = $"Error al guardar la categoría: {ex.Message}";
            }

            return result;
        }

        public async Task<OperationResult> UpdateEntityAsync(Categoria entity)
        {
            var validator = new CategoriaValidator();
            if (!validator.Validate(entity, out string errorMessage))
                return new OperationResult { Success = false, Message = errorMessage };

            var result = new OperationResult();

            try
            {
                var query = @"
                    UPDATE Categoria
                    SET Nombre = @Nombre,
                        Descripcion = @Descripcion,
                        Estado = @Estado,
                        FechaModificacion = GETDATE(),
                        UsuarioModificacion = @UsuarioModificacion
                    WHERE Id = @Id AND IsDeleted = 0";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Id", entity.Id),
                    new SqlParameter("@Nombre", entity.Nombre),
                    new SqlParameter("@Descripcion", entity.Descripcion ?? string.Empty),
                    new SqlParameter("@Estado", entity.Estado),
                    new SqlParameter("@UsuarioModificacion", (object?)entity.UsuarioModificacion ?? DBNull.Value)
                };

                int rows = await SqlHelper.ExecuteNonQueryAsync(_connectionString, query, parameters);

                result.Success = rows > 0;
                result.Message = rows > 0 ? "Categoría actualizada correctamente." : "No se encontró la categoría a actualizar.";
                result.Data = entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la categoría");
                result.Success = false;
                result.Message = $"Error al actualizar la categoría: {ex.Message}";
            }

            return result;
        }

        public async Task<OperationResult> DeleteEntityAsync(Categoria entity)
        {
            var result = new OperationResult();

            try
            {
                // eliminación lógica
                var query = @"
                    UPDATE Categoria
                    SET IsDeleted = 1,
                        FechaEliminacion = GETDATE(),
                        UsuarioEliminacion = @UsuarioEliminacion
                    WHERE Id = @Id AND IsDeleted = 0";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Id", entity.Id),
                    new SqlParameter("@UsuarioEliminacion", (object?)entity.UsuarioEliminacion ?? DBNull.Value)
                };

                int rows = await SqlHelper.ExecuteNonQueryAsync(_connectionString, query, parameters);

                result.Success = rows > 0;
                result.Message = rows > 0 ? "Categoría eliminada correctamente" : "No se encontró la categoría a eliminar.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la categoría");
                result.Success = false;
                result.Message = $"Error al eliminar la categoría: {ex.Message}";
            }

            return result;
        }

        public async Task<OperationResult> RestoreEntityAsync(Categoria entity)
        {
            var result = new OperationResult();

            try
            {
                var query = @"
                    UPDATE Categoria
                    SET IsDeleted = 0,
                        FechaModificacion = GETDATE(),
                        UsuarioModificacion = @UsuarioModificacion
                    WHERE Id = @Id AND IsDeleted = 1";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Id", entity.Id),
                    new SqlParameter("@UsuarioModificacion", (object?)entity.UsuarioModificacion ?? DBNull.Value)
                };

                int rows = await SqlHelper.ExecuteNonQueryAsync(_connectionString, query, parameters);

                result.Success = rows > 0;
                result.Message = rows > 0 ? "Categoría restaurada correctamente." : "No se encontró la categoría para restaurar.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al restaurar la categoría");
                result.Success = false;
                result.Message = $"Error al restaurar la categoría: {ex.Message}";
            }

            return result;
        }

        public async Task<Categoria?> GetEntityByIdAsync(int id)
        {
            Categoria? categoria = null;

            try
            {
                var query = "SELECT * FROM Categoria WHERE Id = @Id AND IsDeleted = 0";
                var parameters = new List<SqlParameter> { new SqlParameter("@Id", id) };

                var lista = await SqlHelper.ExecuteReaderAsync(_connectionString, query, reader =>
                    new Categoria
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? string.Empty : reader.GetString(reader.GetOrdinal("Descripcion")),
                        Estado = reader.GetBoolean(reader.GetOrdinal("Estado")),
                        IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted")),
                        FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FechaCreacion")),
                        FechaModificacion = reader.IsDBNull(reader.GetOrdinal("FechaModificacion")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaModificacion"))
                    }, parameters);

                categoria = lista.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la categoría");
            }

            return categoria;
        }

        public async Task<List<Categoria>> GetAllAsync()
        {
            var lista = new List<Categoria>();

            try
            {
                var query = "SELECT * FROM Categoria WHERE IsDeleted = 0";

                lista = await SqlHelper.ExecuteReaderAsync(_connectionString, query, reader =>
                    new Categoria
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? string.Empty : reader.GetString(reader.GetOrdinal("Descripcion")),
                        Estado = reader.GetBoolean(reader.GetOrdinal("Estado")),
                        IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted")),
                        FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FechaCreacion"))
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las categorías");
            }

            return lista;
        }

        public async Task<bool> ExistsAsync(Expression<Func<Categoria, bool>> filter)
        {
            var categorias = await GetAllAsync();
            return categorias.AsQueryable().Any(filter);
        }

        public Task<OperationResult> GetFilteredAsync(Expression<Func<Categoria, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult> GetCategoriasActivas()
        {
            var result = new OperationResult();

            try
            {
                var query = @"SELECT * 
                      FROM Categoria 
                      WHERE Estado = 1 AND IsDeleted = 0";

                var lista = await SqlHelper.ExecuteReaderAsync(_connectionString, query, reader =>
                    new Categoria
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion"))
                                ? null
                               : reader.GetString(reader.GetOrdinal("Descripcion")),
                        Estado = reader.GetBoolean(reader.GetOrdinal("Estado")),
                        IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted")),
                        FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FechaCreacion"))
                    });

                result.Success = true;
                result.Data = lista;
                result.Message = lista.Any()
                    ? "Categorías activas obtenidas correctamente."
                    : "No se encontraron categorías activas.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categorías activas");
                result.Success = false;
                result.Message = $"Error al obtener categorías activas: {ex.Message}";
            }

            return result;
        }

    }
}
