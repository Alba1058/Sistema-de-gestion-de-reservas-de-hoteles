using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Persistence.Helpers
{
    public static class SqlHelper
    {

        public static async Task<int> ExecuteNonQueryAsync(string connectionString, string query, List<SqlParameter>? parameters = null)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                if (parameters != null)
                    command.Parameters.AddRange(parameters.ToArray());

                await connection.OpenAsync();
                return await command.ExecuteNonQueryAsync();
            }
        }

        public static async Task<object?> ExecuteScalarAsync(string connectionString, string query, List<SqlParameter>? parameters = null)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                if (parameters != null)
                    command.Parameters.AddRange(parameters.ToArray());

                await connection.OpenAsync();
                return await command.ExecuteScalarAsync();
            }
        }

        public static async Task<List<T>> ExecuteReaderAsync<T>(string connectionString, string query, Func<SqlDataReader, T> map, List<SqlParameter>? parameters = null)
        {
            var results = new List<T>();

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                if (parameters != null)
                    command.Parameters.AddRange(parameters.ToArray());

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        results.Add(map(reader));
                    }
                }
            }

            return results;
        }
    }
}
