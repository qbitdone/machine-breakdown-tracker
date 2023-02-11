using Dapper;
using machine_breakdown_tracker.Context;
using machine_breakdown_tracker.Models;
using machine_breakdown_tracker.Services;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace MachineBreakdownTrackerTests
{
    [TestClass]
    public class MachineServiceTests
    {
        private readonly IMachineService _machineService;
        private readonly DapperContext _context;
        private IDbConnection _connection;

        public MachineServiceTests()
        {
            _context = new DapperContext(new ConfigurationBuilder()
                                        .AddJsonFile("appsettings.json")
                                        .Build());

            _machineService = new MachineService(_context);
            _connection = _context.CreateConnection();
        }

        [TestMethod]
        public async Task AddMachine_ValidInput_AddsMachine()
        {
            // Arrange
            var machine = new Machine { Name = "TestMachine1" };

            // Act
            await _machineService.AddMachine(machine);

            // Assert
            var result = await _connection.QuerySingleAsync<Machine>(
                "SELECT * FROM machine WHERE name = @Name",
                new { machine.Name });

            Assert.AreEqual(machine.Name, result.Name);
        }
    }
}