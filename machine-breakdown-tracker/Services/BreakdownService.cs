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

    }
}
