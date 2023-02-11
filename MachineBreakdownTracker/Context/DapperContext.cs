using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;
using System.Data.Common;

namespace machine_breakdown_tracker.Context
{
    public class DapperContext : IDisposable
    {
        private readonly IConfiguration _configuration;
        private string _connectionString;
        private IDbConnection _connection;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                _connectionString = _configuration.GetConnectionString("DefaultConnectionString");
            }

            if (_connection == null)
            {
                _connection = new NpgsqlConnection(_connectionString);

                try
                {
                    _connection.Open();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error: " + ex.Message);
                }
            }

            return _connection;
        }

        public void Dispose()
        {
            if (_connection != null && _connection.State != ConnectionState.Closed)
            {
                _connection.Close();
                _connection.Dispose();
            }
        }
    }
}
