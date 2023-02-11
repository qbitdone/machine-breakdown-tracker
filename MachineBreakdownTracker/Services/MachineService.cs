using Dapper;
using machine_breakdown_tracker.Context;
using machine_breakdown_tracker.Models;
using Npgsql;
using System.Data;
using System.Data.Common;
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

        public async Task<IEnumerable<Machine>> GetAllMachines()
        {
            var sql = "SELECT m.*, b.* FROM machine m LEFT JOIN breakdown b ON m.name = b.machine";

            var machinesAndBreakdowns = await _connection.QueryAsync<Machine, Breakdown, Machine>(sql, (machine, breakdown) =>
                {
                    machine.Breakdowns = machine.Breakdowns ?? new List<Breakdown>();
                    machine.Breakdowns.Add(breakdown);
                    return machine;
                },
                splitOn: "name"
            );

            var machines = new Dictionary<string, Machine>();

            foreach (var machine in machinesAndBreakdowns)
            {
                if (!machines.TryGetValue(machine.Name, out var existingMachine))
                {
                    machines[machine.Name] = machine;
                }
                else
                {
                    existingMachine.Breakdowns.AddRange(machine.Breakdowns);
                }
            }

            return machines.Values;
        }


        public async Task AddMachine(Machine machine)
        {
            if (string.IsNullOrWhiteSpace(machine.Name))
            {
                throw new ArgumentException("Name cannot be null or empty");
            }

            await _connection.ExecuteAsync("INSERT INTO machine (name) VALUES (@Name)", new { machine.Name });
        }
        public async Task<bool> DeleteMachineByName(string machineName) => await _connection.ExecuteAsync("DELETE FROM machine WHERE name = @Name", new { Name = machineName }) > 0;

        public async Task<bool> UpdateMachineByName(string machineName, Machine updatedMachine) => await _connection.ExecuteAsync("UPDATE machine SET name = @Name WHERE name = @OriginalName", new { Name = updatedMachine.Name, OriginalName = machineName }) > 0;

        public async Task<bool> DoesMachineNameExist(string machineName) => await _connection.QueryFirstOrDefaultAsync("SELECT * FROM machine WHERE name = @Name", new { Name = machineName }) > 0;


        public Task<bool> DoesMachineNameExist(Machine machine)
        {
            throw new NotImplementedException();
        }
    }
}
