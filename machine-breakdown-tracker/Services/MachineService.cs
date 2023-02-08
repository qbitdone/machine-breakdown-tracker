using Dapper;
using machine_breakdown_tracker.Models;
using Npgsql;

namespace machine_breakdown_tracker.Services
{
    public class MachineService : IMachineService
    {
        private readonly IConfiguration configuration;

        public MachineService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IEnumerable<Machine>> GetAllMachines()
        {
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnectionString")))
            {
                return await connection.QueryAsync<Machine>("SELECT * FROM machine");
            }
        }
    }
}
