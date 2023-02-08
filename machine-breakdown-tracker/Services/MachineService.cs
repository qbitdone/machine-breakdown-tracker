using Dapper;
using machine_breakdown_tracker.Context;
using machine_breakdown_tracker.Models;
using Npgsql;

namespace machine_breakdown_tracker.Services
{
    public class MachineService : IMachineService
    {
        private readonly DapperContext _context;

        public MachineService(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Machine>> GetAllMachines()
        {
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<Machine>("SELECT * FROM machine");
            }
        }
    }
}
