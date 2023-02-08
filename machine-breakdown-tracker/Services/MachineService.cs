using Dapper;
using machine_breakdown_tracker.Context;
using machine_breakdown_tracker.Models;
using Npgsql;
using System.Data;
using System.Reflection.PortableExecutable;
using Machine = machine_breakdown_tracker.Models.Machine;

namespace machine_breakdown_tracker.Services
{
    public class MachineService : IMachineService
    {
        private readonly DapperContext _context;
        private IDbConnection _connection;

        public MachineService(DapperContext context)
        {
            _context = context;
            _connection = _context.CreateConnection();
        }

        public async Task<IEnumerable<Machine>> GetAllMachines() => await _connection.QueryAsync<Machine>("SELECT * FROM machine");

        public async Task<bool> AddMachine(Machine machine)
        {
            if (string.IsNullOrEmpty(machine.Name) || machine.Name.Length > 100)
            {
                return false;
            }

            var sql = "INSERT INTO machine (name) VALUES (@Name)";
            await _connection.ExecuteAsync(sql, new { machine.Name });
            return true;
        }

        public async Task<bool> DoesMachineNameExist(Machine machine)
        {
            var existingMachine = await _connection.QueryFirstOrDefaultAsync<Machine>("SELECT * FROM machine WHERE name = @Name", new { machine.Name });
            if (existingMachine != null)
            {
                return false;
            }
            return true;
        }

    }
}
