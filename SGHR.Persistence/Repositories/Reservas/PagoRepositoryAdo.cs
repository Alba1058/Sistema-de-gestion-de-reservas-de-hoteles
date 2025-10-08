using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Validators;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Reservas;
using SGHR.Domain.Repository;
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
    public class PagoRepositoryADO : IPagoRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PagoRepositoryADO> _logger;
        private readonly string _connectionString;

        public PagoRepositoryADO(IConfiguration configuration, ILogger<PagoRepositoryADO> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _connectionString = _configuration.GetConnectionString("SghrConnString");
        }

        public async Task<OperationResult> SaveEntityAsync(Pago entity)
        {
            var validator = new PagoValidator();
            if (!validator.Validate(entity, out string errorMessage))
                return new OperationResult { Success = false, Message = errorMessage };

            var result = new OperationResult();

            try
            {
                var query = @"
                    INSERT INTO Pago 
                    (IdReserva, Monto, FechaPago, Metodo, Confirmado, Estado, IsDeleted, FechaCreacion, UsuarioCreacion)
                    VALUES (@IdReserva, @Monto, @FechaPago, @Metodo, @Confirmado, @Estado, 0, GETDATE(), @UsuarioCreacion)";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@IdReserva", entity.IdReserva),
                    new SqlParameter("@Monto", entity.Monto),
                    new SqlParameter("@FechaPago", entity.FechaPago),
                    new SqlParameter("@Metodo", entity.Metodo),
                    new SqlParameter("@Confirmado", entity.Confirmado),
                    new SqlParameter("@Estado", entity.Estado),
                    new SqlParameter("@UsuarioCreacion", (object?)entity.UsuarioCreacion ?? DBNull.Value)
                };

                int rows = await SqlHelper.ExecuteNonQueryAsync(_connectionString, query, parameters);

                result.Success = rows > 0;
                result.Message = rows > 0 ? "Pago guardado correctamente." : "No se pudo guardar el pago.";
                result.Data = entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar el pago");
                result.Success = false;
                result.Message = $"Error al guardar el pago: {ex.Message}";
            }

            return result;
        }

        public async Task<OperationResult> UpdateEntityAsync(Pago entity)
        {
            var validator = new PagoValidator();
            if (!validator.Validate(entity, out string errorMessage))
                return new OperationResult { Success = false, Message = errorMessage };

            var result = new OperationResult();

            try
            {
                var query = @"
                    UPDATE Pago
                    SET IdReserva = @IdReserva,
                        Monto = @Monto,
                        FechaPago = @FechaPago,
                        Metodo = @Metodo,
                        Confirmado = @Confirmado,
                        Estado = @Estado,
                        FechaModificacion = GETDATE(),
                        UsuarioModificacion = @UsuarioModificacion
                    WHERE Id = @Id AND IsDeleted = 0";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Id", entity.Id),
                    new SqlParameter("@IdReserva", entity.IdReserva),
                    new SqlParameter("@Monto", entity.Monto),
                    new SqlParameter("@FechaPago", entity.FechaPago),
                    new SqlParameter("@Metodo", entity.Metodo),
                    new SqlParameter("@Confirmado", entity.Confirmado),
                    new SqlParameter("@Estado", entity.Estado),
                    new SqlParameter("@UsuarioModificacion", (object?)entity.UsuarioModificacion ?? DBNull.Value)
                };

                int rows = await SqlHelper.ExecuteNonQueryAsync(_connectionString, query, parameters);

                result.Success = rows > 0;
                result.Message = rows > 0 ? "Pago actualizado correctamente." : "No se encontró el pago a actualizar.";
                result.Data = entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el pago");
                result.Success = false;
                result.Message = $"Error al actualizar el pago: {ex.Message}";
            }

            return result;
        }

        public async Task<OperationResult> DeleteEntityAsync(Pago entity)
        {
            var result = new OperationResult();

            try
            {
                var query = @"
                    UPDATE Pago
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
                result.Message = rows > 0 ? "Pago eliminado correctamente." : "No se encontró el pago a eliminar.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el pago");
                result.Success = false;
                result.Message = $"Error al eliminar el pago: {ex.Message}";
            }

            return result;
        }

        public async Task<OperationResult> RestoreEntityAsync(Pago entity)
        {
            var result = new OperationResult();

            try
            {
                var query = @"
                    UPDATE Pago
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
                result.Message = rows > 0 ? "Pago restaurado correctamente." : "No se encontró el pago para restaurar.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al restaurar el pago");
                result.Success = false;
                result.Message = $"Error al restaurar el pago: {ex.Message}";
            }

            return result;
        }

        public async Task<Pago?> GetEntityByIdAsync(int id)
        {
            Pago? pago = null;

            try
            {
                var query = "SELECT * FROM Pago WHERE Id = @Id AND IsDeleted = 0";
                var parameters = new List<SqlParameter> { new SqlParameter("@Id", id) };

                var lista = await SqlHelper.ExecuteReaderAsync(_connectionString, query, reader =>
                    new Pago
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        IdReserva = reader.GetInt32(reader.GetOrdinal("IdReserva")),
                        Monto = reader.GetDecimal(reader.GetOrdinal("Monto")),
                        FechaPago = reader.GetDateTime(reader.GetOrdinal("FechaPago")),
                        Metodo = reader.GetString(reader.GetOrdinal("Metodo")),
                        Confirmado = reader.GetBoolean(reader.GetOrdinal("Confirmado")),
                        Estado = reader.GetBoolean(reader.GetOrdinal("Estado")),
                        IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted")),
                        FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FechaCreacion")),
                        FechaModificacion = reader.IsDBNull(reader.GetOrdinal("FechaModificacion")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaModificacion"))
                    }, parameters);

                pago = lista.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el pago por ID");
            }

            return pago;
        }

        public async Task<List<Pago>> GetAllAsync()
        {
            var lista = new List<Pago>();

            try
            {
                var query = "SELECT * FROM Pago WHERE IsDeleted = 0";

                lista = await SqlHelper.ExecuteReaderAsync(_connectionString, query, reader =>
                    new Pago
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        IdReserva = reader.GetInt32(reader.GetOrdinal("IdReserva")),
                        Monto = reader.GetDecimal(reader.GetOrdinal("Monto")),
                        FechaPago = reader.GetDateTime(reader.GetOrdinal("FechaPago")),
                        Metodo = reader.GetString(reader.GetOrdinal("Metodo")),
                        Confirmado = reader.GetBoolean(reader.GetOrdinal("Confirmado")),
                        Estado = reader.GetBoolean(reader.GetOrdinal("Estado")),
                        IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted")),
                        FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FechaCreacion"))
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los pagos");
            }

            return lista;
        }

        public async Task<bool> ExistsAsync(Expression<Func<Pago, bool>> filter)
        {
            var pagos = await GetAllAsync();
            return pagos.AsQueryable().Any(filter);
        }

        public Task<OperationResult> GetFilteredAsync(Expression<Func<Pago, bool>> filter)
        {
            throw new NotImplementedException();
        }
    }
}