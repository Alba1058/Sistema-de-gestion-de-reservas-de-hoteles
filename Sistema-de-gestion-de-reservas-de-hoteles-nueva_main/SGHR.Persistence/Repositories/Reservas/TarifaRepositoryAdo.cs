using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Reservas;
using SGHR.Persistence.Helpers;
using SGHR.Persistence.Interfaces.Reservas;
using System.Data;
using System.Linq.Expressions;

namespace SGHR.Persistence.Repositories.Reservas
{
    public sealed class TarifaRepositoryAdo : ITarifaRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<TarifaRepositoryAdo> _logger;

        public TarifaRepositoryAdo(IConfiguration configuration, ILogger<TarifaRepositoryAdo> logger)
        {
            _connectionString = configuration.GetConnectionString("SghrConnString")
                                ?? throw new ArgumentNullException(nameof(configuration), "La cadena de conexión 'SghrConnString' no puede ser nula.");
            _logger = logger;
        }

        private Tarifa MapToTarifa(SqlDataReader reader)
        {
            return new Tarifa
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Tipo = reader["Tipo"] as string ?? string.Empty,
                Monto = reader.GetDecimal(reader.GetOrdinal("Monto")),
                PrecioPorNoche = reader.GetDecimal(reader.GetOrdinal("PrecioPorNoche")),
                Descuento = reader.GetDecimal(reader.GetOrdinal("Descuento")),
                Descripcion = reader["Descripcion"] as string ?? string.Empty,
                IdHabitacion = reader.GetInt32(reader.GetOrdinal("IdHabitacion")),
                FechaInicio = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("FechaInicio"))),
                FechaFin = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("FechaFin"))),
                Estado = reader["Estado"] != DBNull.Value && Convert.ToBoolean(reader["Estado"]),
                IsDeleted = reader["IsDeleted"] != DBNull.Value && Convert.ToBoolean(reader["IsDeleted"]),
                FechaCreacion = reader["FechaCreacion"] != DBNull.Value ? Convert.ToDateTime(reader["FechaCreacion"]) : default,
                FechaModificacion = reader["FechaModificacion"] != DBNull.Value ? Convert.ToDateTime(reader["FechaModificacion"]) : default
            };
        }
        public async Task<OperationResult<Tarifa>> SaveEntityAsync(Tarifa entity)
        {
            try
            {
                var query = @"
                    INSERT INTO Tarifa (Tipo, Monto, FechaInicio, FechaFin, PrecioPorNoche, Descuento, Descripcion, IdHabitacion, Estado, IsDeleted, FechaCreacion)
                    VALUES (@Tipo, @Monto, @FechaInicio, @FechaFin, @PrecioPorNoche, @Descuento, @Descripcion, @IdHabitacion, @Estado, @IsDeleted, @FechaCreacion);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Tipo", SqlDbType.NVarChar) { Value = (object?)entity.Tipo ?? DBNull.Value },
                    new SqlParameter("@Monto", SqlDbType.Decimal) { Value = entity.Monto },
                    new SqlParameter("@FechaInicio", SqlDbType.DateTime) { Value = entity.FechaInicio.ToDateTime(TimeOnly.MinValue) },
                    new SqlParameter("@FechaFin", SqlDbType.DateTime) { Value = entity.FechaFin.ToDateTime(TimeOnly.MinValue) },
                    new SqlParameter("@PrecioPorNoche", SqlDbType.Decimal) { Value = entity.PrecioPorNoche },
                    new SqlParameter("@Descuento", SqlDbType.Decimal) { Value = entity.Descuento },
                    new SqlParameter("@Descripcion", SqlDbType.NVarChar) { Value = (object?)entity.Descripcion ?? DBNull.Value },
                    new SqlParameter("@IdHabitacion", SqlDbType.Int) { Value = entity.IdHabitacion },
                    new SqlParameter("@Estado", SqlDbType.Bit) { Value = entity.Estado },
                    new SqlParameter("@IsDeleted", SqlDbType.Bit) { Value = false },
                    new SqlParameter("@FechaCreacion", SqlDbType.DateTime) { Value = DateTime.UtcNow }
                };

                var scalar = await SqlHelper.ExecuteScalarAsync(_connectionString, query, parameters);
                if (scalar != null && int.TryParse(scalar.ToString(), out var newId))
                {
                    entity.Id = newId;
                    _logger.LogInformation("Tarifa insertada con Id {Id}", newId);
                    return OperationResult<Tarifa>.Ok(entity, "Tarifa creada correctamente.");
                }

                _logger.LogError("No se pudo insertar la tarifa; ExecuteScalar devolvió nulo.");
                return OperationResult<Tarifa>.Fail("No se pudo crear la tarifa.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar tarifa.");
                return OperationResult<Tarifa>.Fail($"Error interno al crear la tarifa: {ex.Message}");
            }
        }

        public async Task<OperationResult<Tarifa>> UpdateEntityAsync(Tarifa entity)
        {
            try
            {

                var query = @"
                    UPDATE Tarifa
                    SET Tipo = @Tipo, Monto = @Monto, FechaInicio = @FechaInicio, FechaFin = @FechaFin, 
                        PrecioPorNoche = @PrecioPorNoche, Descuento = @Descuento, Descripcion = @Descripcion, 
                        IdHabitacion = @IdHabitacion, Estado = @Estado, IsDeleted = @IsDeleted, 
                        FechaModificacion = @FechaModificacion
                    WHERE Id = @Id";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Tipo", SqlDbType.NVarChar) { Value = (object?)entity.Tipo ?? DBNull.Value },
                    new SqlParameter("@Monto", SqlDbType.Decimal) { Value = entity.Monto },
                    new SqlParameter("@FechaInicio", SqlDbType.DateTime) { Value = entity.FechaInicio.ToDateTime(TimeOnly.MinValue) },
                    new SqlParameter("@FechaFin", SqlDbType.DateTime) { Value = entity.FechaFin.ToDateTime(TimeOnly.MinValue) },
                    new SqlParameter("@PrecioPorNoche", SqlDbType.Decimal) { Value = entity.PrecioPorNoche },
                    new SqlParameter("@Descuento", SqlDbType.Decimal) { Value = entity.Descuento },
                    new SqlParameter("@Descripcion", SqlDbType.NVarChar) { Value = (object?)entity.Descripcion ?? DBNull.Value },
                    new SqlParameter("@IdHabitacion", SqlDbType.Int) { Value = entity.IdHabitacion },
                    new SqlParameter("@Estado", SqlDbType.Bit) { Value = entity.Estado },
                    new SqlParameter("@IsDeleted", SqlDbType.Bit) { Value = entity.IsDeleted },
                    new SqlParameter("@FechaModificacion", SqlDbType.DateTime) { Value = DateTime.UtcNow },
                    new SqlParameter("@Id", SqlDbType.Int) { Value = entity.Id }
                };

                var affected = await SqlHelper.ExecuteNonQueryAsync(_connectionString, query, parameters);
                if (affected > 0)
                {
                    _logger.LogInformation("Tarifa {Id} actualizada.", entity.Id);
                    return OperationResult<Tarifa>.Ok(entity, "Tarifa actualizada correctamente.");
                }

                _logger.LogWarning("No se actualizó la tarifa {Id} (filas afectadas = 0).", entity.Id);
                return OperationResult<Tarifa>.Fail("No se encontró la tarifa a actualizar.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar tarifa {Id}.", entity.Id);
                return OperationResult<Tarifa>.Fail("Error interno al actualizar la tarifa.");
            }
        }

        public async Task<OperationResult<bool>> DeleteEntityAsync(Tarifa entity)
        {
            try
            {
                var query = @"UPDATE Tarifa 
                      SET IsDeleted = 1, FechaModificacion = @FechaModificacion 
                      WHERE Id = @Id AND IsDeleted = 0";

                var parameters = new List<SqlParameter>
        {
            new SqlParameter("@FechaModificacion", SqlDbType.DateTime) { Value = DateTime.UtcNow },
            new SqlParameter("@Id", SqlDbType.Int) { Value = entity.Id }
        };

                var affected = await SqlHelper.ExecuteNonQueryAsync(_connectionString, query, parameters);

                if (affected > 0)
                {
                    _logger.LogInformation("Tarifa {Id} eliminada lógicamente.", entity.Id);
                    return OperationResult<bool>.Ok(true, "Entidad eliminada correctamente.");
                }

                return OperationResult<bool>.Fail("No se encontró la tarifa a eliminar.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar tarifa {Id}.", entity.Id);
                return OperationResult<bool>.Fail($"Error al eliminar: {ex.Message}");
            }
        }

        public async Task<OperationResult<bool>> RestoreEntityAsync(Tarifa entity)
        {
            try
            {
                var query = @"UPDATE Tarifa 
                      SET IsDeleted = 0, FechaModificacion = @FechaModificacion 
                      WHERE Id = @Id AND IsDeleted = 1";

                var parameters = new List<SqlParameter>
        {
            new SqlParameter("@FechaModificacion", SqlDbType.DateTime) { Value = DateTime.UtcNow },
            new SqlParameter("@Id", SqlDbType.Int) { Value = entity.Id }
        };

                var affected = await SqlHelper.ExecuteNonQueryAsync(_connectionString, query, parameters);

                if (affected > 0)
                {
                    _logger.LogInformation("Tarifa {Id} restaurada.", entity.Id);
                    return OperationResult<bool>.Ok(true, "Entidad restaurada correctamente.");
                }

                return OperationResult<bool>.Fail("No se encontró la tarifa a restaurar.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al restaurar tarifa {Id}.", entity.Id);
                return OperationResult<bool>.Fail($"Error al restaurar: {ex.Message}");
            }
        }

        public async Task<List<Tarifa>> GetAllAsync()
        {
            var query = "SELECT * FROM Tarifa WHERE IsDeleted = 0";
            try
            {
                return await SqlHelper.ExecuteReaderAsync(_connectionString, query, MapToTarifa);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las tarifas.");
                return new List<Tarifa>();
            }
        }

        public async Task<Tarifa?> GetEntityByIdAsync(int id)
        {
            var query = "SELECT * FROM Tarifa WHERE Id = @Id AND IsDeleted = 0";
            var parameters = new List<SqlParameter> { new SqlParameter("@Id", SqlDbType.Int) { Value = id } };
            try
            {
                var result = await SqlHelper.ExecuteReaderAsync(_connectionString, query, MapToTarifa, parameters);
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tarifa por ID.");
                return null;
            }
        }

        public async Task<OperationResult<List<Tarifa>>> GetFilteredAsync(Expression<Func<Tarifa, bool>> filter)
        {
            try
            {
                var all = await GetAllAsync();
                var filtered = all.AsQueryable().Where(filter).ToList();
                return OperationResult<List<Tarifa>>.Ok(filtered);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tarifas filtradas.");
                return OperationResult<List<Tarifa>>.Fail($"Error: {ex.Message}");
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<Tarifa, bool>> filter)
        {
            try
            {
                var all = await GetAllAsync();
                return all.AsQueryable().Any(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar existencia de tarifa.");
                return false;
            }
        }
    }
}