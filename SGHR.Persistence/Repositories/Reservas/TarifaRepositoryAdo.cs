using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Validators;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Reservas;
using SGHR.Persistence.Helpers;
using SGHR.Persistence.Interfaces.Reservas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Persistence.Repositories.Reservas
{
    public class TarifaRepositoryADO : ITarifaRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TarifaRepositoryADO> _logger;
        private readonly string _connectionString;

        public TarifaRepositoryADO(IConfiguration configuration, ILogger<TarifaRepositoryADO> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _connectionString = _configuration.GetConnectionString("SghrConnString");
        }

        public async Task<OperationResult> SaveEntityAsync(Tarifa entity)
        {
            var validator = new TarifaValidator();
            if (!validator.Validate(entity, out string errorMessage))
                return new OperationResult { Success = false, Message = errorMessage };

            var result = new OperationResult();

            try
            {
                var query = @"
                    INSERT INTO Tarifa 
                    (Tipo, Monto, FechaInicio, FechaFin, PrecioPorNoche, Descuento, Descripcion, IdHabitacion, 
                     Estado, IsDeleted, FechaCreacion, UsuarioCreacion)
                    VALUES (@Tipo, @Monto, @FechaInicio, @FechaFin, @PrecioPorNoche, @Descuento, @Descripcion, 
                            @IdHabitacion, @Estado, 0, GETDATE(), @UsuarioCreacion)";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Tipo", entity.Tipo),
                    new SqlParameter("@Monto", entity.Monto),
                    new SqlParameter("@FechaInicio", entity.FechaInicio.ToDateTime(TimeOnly.MinValue)),
                    new SqlParameter("@FechaFin", entity.FechaFin.ToDateTime(TimeOnly.MinValue)),
                    new SqlParameter("@PrecioPorNoche", entity.PrecioPorNoche),
                    new SqlParameter("@Descuento", entity.Descuento),
                    new SqlParameter("@Descripcion", entity.Descripcion),
                    new SqlParameter("@IdHabitacion", entity.IdHabitacion),
                    new SqlParameter("@Estado", entity.Estado),
                    new SqlParameter("@UsuarioCreacion", (object?)entity.UsuarioCreacion ?? DBNull.Value)
                };

                int rows = await SqlHelper.ExecuteNonQueryAsync(_connectionString, query, parameters);

                result.Success = rows > 0;
                result.Message = rows > 0 ? "Tarifa guardada correctamente." : "No se pudo guardar la tarifa.";
                result.Data = entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la tarifa");
                result.Success = false;
                result.Message = $"Error al guardar la tarifa: {ex.Message}";
            }

            return result;
        }

        public async Task<OperationResult> UpdateEntityAsync(Tarifa entity)
        {
            var validator = new TarifaValidator();
            if (!validator.Validate(entity, out string errorMessage))
                return new OperationResult { Success = false, Message = errorMessage };

            var result = new OperationResult();

            try
            {
                var query = @"
                    UPDATE Tarifa
                    SET Tipo = @Tipo,
                        Monto = @Monto,
                        FechaInicio = @FechaInicio,
                        FechaFin = @FechaFin,
                        PrecioPorNoche = @PrecioPorNoche,
                        Descuento = @Descuento,
                        Descripcion = @Descripcion,
                        IdHabitacion = @IdHabitacion,
                        Estado = @Estado,
                        FechaModificacion = GETDATE(),
                        UsuarioModificacion = @UsuarioModificacion
                    WHERE Id = @Id AND IsDeleted = 0";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Id", entity.Id),
                    new SqlParameter("@Tipo", entity.Tipo),
                    new SqlParameter("@Monto", entity.Monto),
                    new SqlParameter("@FechaInicio", entity.FechaInicio.ToDateTime(TimeOnly.MinValue)),
                    new SqlParameter("@FechaFin", entity.FechaFin.ToDateTime(TimeOnly.MinValue)),
                    new SqlParameter("@PrecioPorNoche", entity.PrecioPorNoche),
                    new SqlParameter("@Descuento", entity.Descuento),
                    new SqlParameter("@Descripcion", entity.Descripcion),
                    new SqlParameter("@IdHabitacion", entity.IdHabitacion),
                    new SqlParameter("@Estado", entity.Estado),
                    new SqlParameter("@UsuarioModificacion", (object?)entity.UsuarioModificacion ?? DBNull.Value)
                };

                int rows = await SqlHelper.ExecuteNonQueryAsync(_connectionString, query, parameters);

                result.Success = rows > 0;
                result.Message = rows > 0 ? "Tarifa actualizada correctamente." : "No se encontró la tarifa a actualizar.";
                result.Data = entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la tarifa");
                result.Success = false;
                result.Message = $"Error al actualizar la tarifa: {ex.Message}";
            }

            return result;
        }

        public async Task<OperationResult> DeleteEntityAsync(Tarifa entity)
        {
            var result = new OperationResult();

            try
            {
                var query = @"
                    UPDATE Tarifa
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
                result.Message = rows > 0 ? "Tarifa eliminada correctamente" : "No se encontró la tarifa a eliminar.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la tarifa");
                result.Success = false;
                result.Message = $"Error al eliminar la tarifa: {ex.Message}";
            }

            return result;
        }

        public async Task<OperationResult> RestoreEntityAsync(Tarifa entity)
        {
            var result = new OperationResult();

            try
            {
                var query = @"
                    UPDATE Tarifa
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
                result.Message = rows > 0 ? "Tarifa restaurada correctamente." : "No se encontró la tarifa para restaurar.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al restaurar la tarifa");
                result.Success = false;
                result.Message = $"Error al restaurar la tarifa: {ex.Message}";
            }

            return result;
        }

        public async Task<Tarifa?> GetEntityByIdAsync(int id)
        {
            Tarifa? tarifa = null;

            try
            {
                var query = "SELECT * FROM Tarifa WHERE Id = @Id AND IsDeleted = 0";
                var parameters = new List<SqlParameter> { new SqlParameter("@Id", id) };

                var lista = await SqlHelper.ExecuteReaderAsync(_connectionString, query, reader =>
                    new Tarifa
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Tipo = reader.GetString(reader.GetOrdinal("Tipo")),
                        Monto = reader.GetDecimal(reader.GetOrdinal("Monto")),
                        FechaInicio = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("FechaInicio"))),
                        FechaFin = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("FechaFin"))),
                        PrecioPorNoche = reader.GetDecimal(reader.GetOrdinal("PrecioPorNoche")),
                        Descuento = reader.GetDecimal(reader.GetOrdinal("Descuento")),
                        Descripcion = reader.GetString(reader.GetOrdinal("Descripcion")),
                        IdHabitacion = reader.GetInt32(reader.GetOrdinal("IdHabitacion")),
                        Estado = reader.GetBoolean(reader.GetOrdinal("Estado")),
                        IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted")),
                        FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FechaCreacion"))
                    }, parameters);

                tarifa = lista.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la tarifa por ID");
            }

            return tarifa;
        }

        public async Task<List<Tarifa>> GetAllAsync()
        {
            var lista = new List<Tarifa>();

            try
            {
                var query = "SELECT * FROM Tarifa WHERE IsDeleted = 0";

                lista = await SqlHelper.ExecuteReaderAsync(_connectionString, query, reader =>
                    new Tarifa
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Tipo = reader.GetString(reader.GetOrdinal("Tipo")),
                        Monto = reader.GetDecimal(reader.GetOrdinal("Monto")),
                        FechaInicio = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("FechaInicio"))),
                        FechaFin = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("FechaFin"))),
                        PrecioPorNoche = reader.GetDecimal(reader.GetOrdinal("PrecioPorNoche")),
                        Descuento = reader.GetDecimal(reader.GetOrdinal("Descuento")),
                        Descripcion = reader.GetString(reader.GetOrdinal("Descripcion")),
                        IdHabitacion = reader.GetInt32(reader.GetOrdinal("IdHabitacion")),
                        Estado = reader.GetBoolean(reader.GetOrdinal("Estado")),
                        IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted"))
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las tarifas");
            }

            return lista;
        }

        public async Task<bool> ExistsAsync(Expression<Func<Tarifa, bool>> filter)
        {
            var tarifas = await GetAllAsync();
            return tarifas.AsQueryable().Any(filter);
        }

        public Task<OperationResult> GetFilteredAsync(Expression<Func<Tarifa, bool>> filter)
        {
            throw new NotImplementedException();
        }
    }
}