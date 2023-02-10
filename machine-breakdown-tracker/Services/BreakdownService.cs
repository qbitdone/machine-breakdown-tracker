using Dapper;
using machine_breakdown_tracker.Context;
using machine_breakdown_tracker.Models;
using System.Data;

namespace machine_breakdown_tracker.Services
{
    public class BreakdownService : IBreakdownService
    {
        private readonly DapperContext _context;
        private IDbConnection _connection;

        public BreakdownService(DapperContext context)
        {
            _context = context;
            _connection = _context.CreateConnection();
        }
        public async Task<IEnumerable<Breakdown>> GetAllBreakdowns() => await _connection.QueryAsync<Breakdown>("SELECT * FROM breakdown");

        public async Task<bool> AddBreakdown(BreakdownRequest breakdown)
        {
            if (string.IsNullOrEmpty(breakdown.Name) || breakdown.Name.Length > 100 ||
                string.IsNullOrEmpty(breakdown.Machine) || breakdown.Machine.Length > 100 ||
                string.IsNullOrEmpty(breakdown.Description) || breakdown.Description.Length > 1000 ||
                breakdown.StartTime == null || breakdown.EndTime == null)
            {
                return false;
            }

            await _connection.ExecuteAsync("INSERT INTO breakdown (name, machine, priority, start_time, end_time, description, eliminated) " +
                                           "VALUES (@Name, @Machine, @Priority, @StartTime, @EndTime, @Description, @Eliminated)",
                                           breakdown);

            return true;
        }
    }
}
