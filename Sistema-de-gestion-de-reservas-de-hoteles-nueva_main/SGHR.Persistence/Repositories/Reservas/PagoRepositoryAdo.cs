using Microsoft.Data.SqlClient;
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
    public sealed class PagoRepositoryAdo : IPagoRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<PagoRepositoryAdo> _logger;

        public PagoRepositoryAdo(IConfiguration configuration, ILogger<PagoRepositoryAdo> logger)
        {
            _connectionString = configuration.GetConnectionString("SghrConnString")
                ?? throw new ArgumentNullException("La cadena de conexión 'SghrConnString' no puede ser nula.");
            _logger = logger;
        }

        private static Pago MapToPago(SqlDataReader reader)
        {
            return new Pago
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                IdReserva = reader.GetInt32(reader.GetOrdinal("IdReserva")),
                Monto = reader.GetDecimal(reader.GetOrdinal("Monto")),
                FechaPago = reader.GetDateTime(reader.GetOrdinal("FechaPago")),
                Metodo = reader["Metodo"] as string ?? string.Empty,
                Confirmado = reader.GetBoolean(reader.GetOrdinal("Confirmado")),
                Estado = reader["Estado"] != DBNull.Value && Convert.ToBoolean(reader["Estado"]),
                IsDeleted = reader["IsDeleted"] != DBNull.Value && Convert.ToBoolean(reader["IsDeleted"]),
                FechaCreacion = reader["FechaCreacion"] != DBNull.Value ? Convert.ToDateTime(reader["FechaCreacion"]) : default,
                FechaModificacion = reader["FechaModificacion"] != DBNull.Value ? Convert.ToDateTime(reader["FechaModificacion"]) : default
            };
        }

        public async Task<OperationResult<Pago>> SaveEntityAsync(Pago entity)
        {
            try
            {
                const string query = @"
                    INSERT INTO Pago (IdReserva, Monto, FechaPago, Metodo, Confirmado, Estado, IsDeleted, FechaCreacion)
                    VALUES (@IdReserva, @Monto, @FechaPago, @Metodo, @Confirmado, @Estado, @IsDeleted, @FechaCreacion);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                var parameters = new List<SqlParameter>
                {
                    new("@IdReserva", SqlDbType.Int) { Value = entity.IdReserva },
                    new("@Monto", SqlDbType.Decimal) { Value = entity.Monto },
                    new("@FechaPago", SqlDbType.DateTime) { Value = entity.FechaPago == default ? DateTime.UtcNow : entity.FechaPago },
                    new("@Metodo", SqlDbType.NVarChar) { Value = (object?)entity.Metodo ?? DBNull.Value },
                    new("@Confirmado", SqlDbType.Bit) { Value = entity.Confirmado },
                    new("@Estado", SqlDbType.Bit) { Value = entity.Estado },
                    new("@IsDeleted", SqlDbType.Bit) { Value = false },
                    new("@FechaCreacion", SqlDbType.DateTime) { Value = DateTime.UtcNow }
                };

                var scalar = await SqlHelper.ExecuteScalarAsync(_connectionString, query, parameters);
                if (scalar != null && int.TryParse(scalar.ToString(), out var newId))
                {
                    entity.Id = newId;
                    _logger.LogInformation("Pago insertado con Id {Id}", newId);
                    return OperationResult<Pago>.Ok(entity, "Pago creado correctamente.");
                }

                return OperationResult<Pago>.Fail("No se pudo crear el pago.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar pago.");
                return OperationResult<Pago>.Fail("Error interno al crear el pago.");
            }
        }

 
        public async Task<OperationResult<Pago>> UpdateEntityAsync(Pago entity)
        {
            try
            {
                const string query = @"
                    UPDATE Pago
                    SET IdReserva = @IdReserva, Monto = @Monto, FechaPago = @FechaPago, Metodo = @Metodo, 
                        Confirmado = @Confirmado, Estado = @Estado, IsDeleted = @IsDeleted, 
                        FechaModificacion = @FechaModificacion
                    WHERE Id = @Id";

                var parameters = new List<SqlParameter>
                {
                    new("@IdReserva", SqlDbType.Int) { Value = entity.IdReserva },
                    new("@Monto", SqlDbType.Decimal) { Value = entity.Monto },
                    new("@FechaPago", SqlDbType.DateTime) { Value = entity.FechaPago },
                    new("@Metodo", SqlDbType.NVarChar) { Value = (object?)entity.Metodo ?? DBNull.Value },
                    new("@Confirmado", SqlDbType.Bit) { Value = entity.Confirmado },
                    new("@Estado", SqlDbType.Bit) { Value = entity.Estado },
                    new("@IsDeleted", SqlDbType.Bit) { Value = entity.IsDeleted },
                    new("@FechaModificacion", SqlDbType.DateTime) { Value = DateTime.UtcNow },
                    new("@Id", SqlDbType.Int) { Value = entity.Id }
                };

                var affected = await SqlHelper.ExecuteNonQueryAsync(_connectionString, query, parameters);
                if (affected > 0)
                    return OperationResult<Pago>.Ok(entity, "Pago actualizado correctamente.");

                return OperationResult<Pago>.Fail("No se encontró el pago a actualizar.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar pago {Id}.", entity.Id);
                return OperationResult<Pago>.Fail("Error interno al actualizar el pago.");
            }
        }

        public async Task<OperationResult<bool>> DeleteEntityAsync(Pago entity)
        {
            try
            {
                const string query = @"UPDATE Pago 
                               SET IsDeleted = 1, FechaModificacion = @FechaModificacion 
                               WHERE Id = @Id AND IsDeleted = 0";

                var parameters = new List<SqlParameter>
        {
            new("@FechaModificacion", SqlDbType.DateTime) { Value = DateTime.UtcNow },
            new("@Id", SqlDbType.Int) { Value = entity.Id }
        };

                var affected = await SqlHelper.ExecuteNonQueryAsync(_connectionString, query, parameters);
                if (affected > 0)
                {
                    _logger.LogInformation("Pago {Id} eliminado lógicamente.", entity.Id);
                    return OperationResult<bool>.Ok(true, "Entidad eliminada correctamente.");
                }

                return OperationResult<bool>.Fail("No se encontró el pago a eliminar.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar pago {Id}.", entity.Id);
                return OperationResult<bool>.Fail($"Error al eliminar: {ex.Message}");
            }
        }

        public async Task<OperationResult<bool>> RestoreEntityAsync(Pago entity)
        {
            try
            {
                const string query = @"UPDATE Pago 
                               SET IsDeleted = 0, FechaModificacion = @FechaModificacion 
                               WHERE Id = @Id AND IsDeleted = 1";

                var parameters = new List<SqlParameter>
        {
            new("@FechaModificacion", SqlDbType.DateTime) { Value = DateTime.UtcNow },
            new("@Id", SqlDbType.Int) { Value = entity.Id }
        };

                var affected = await SqlHelper.ExecuteNonQueryAsync(_connectionString, query, parameters);
                if (affected > 0)
                {
                    _logger.LogInformation("Pago {Id} restaurado.", entity.Id);
                    return OperationResult<bool>.Ok(true, "Entidad restaurada correctamente.");
                }

                return OperationResult<bool>.Fail("No se encontró el pago a restaurar.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al restaurar pago {Id}.", entity.Id);
                return OperationResult<bool>.Fail($"Error al restaurar: {ex.Message}");
            }
        }


        public async Task<List<Pago>> GetAllAsync()
        {
            const string query = @"
                SELECT Id, IdReserva, Monto, FechaPago, Metodo, Confirmado, Estado, IsDeleted, FechaCreacion, FechaModificacion 
                FROM Pago WHERE IsDeleted = 0";

            try
            {
                return await SqlHelper.ExecuteReaderAsync(_connectionString, query, MapToPago);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los pagos.");
                return new List<Pago>();
            }
        }

        public async Task<Pago?> GetEntityByIdAsync(int id)
        {
            const string query = @"
                SELECT Id, IdReserva, Monto, FechaPago, Metodo, Confirmado, Estado, IsDeleted, FechaCreacion, FechaModificacion 
                FROM Pago WHERE Id = @Id AND IsDeleted = 0";

            var parameters = new List<SqlParameter> { new("@Id", SqlDbType.Int) { Value = id } };

            try
            {
                var result = await SqlHelper.ExecuteReaderAsync(_connectionString, query, MapToPago, parameters);
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pago por ID.");
                return null;
            }
        }

        public async Task<OperationResult<List<Pago>>> GetFilteredAsync(Expression<Func<Pago, bool>> filter)
        {
            try
            {
                var all = await GetAllAsync();
                var filtered = all.AsQueryable().Where(filter).ToList();
                return OperationResult<List<Pago>>.Ok(filtered);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pagos filtrados.");
                return OperationResult<List<Pago>>.Fail($"Error: {ex.Message}");
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<Pago, bool>> filter)
        {
            try
            {
                var all = await GetAllAsync();
                return all.AsQueryable().Any(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar existencia de pago.");
                return false;
            }
        }
    }
}
