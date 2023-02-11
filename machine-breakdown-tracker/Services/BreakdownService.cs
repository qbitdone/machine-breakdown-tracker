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
                breakdown.StartTime == null)
            {
                return false;
            }

            await _connection.ExecuteAsync("INSERT INTO breakdown (name, machine, priority, start_time, end_time, description, eliminated) " +
                                           "VALUES (@Name, @Machine, @Priority, @StartTime, @EndTime, @Description, @Eliminated)",
                                           breakdown);

            return true;
        }

        public async Task<bool> UpdateBreakdownById(Guid breakdownId, BreakdownRequest updatedBreakdown)
        {
            if (!await IsValidPriority(updatedBreakdown.Priority))
            {
                throw new ArgumentException("Invalid priority value");
            }

            var breakdown = await GetBreakdownById(breakdownId);

            if (await GetBreakdownById(breakdownId) == default(Breakdown))
            {
                return false;
            }

            var rowsAffected = await _connection.ExecuteAsync(@"UPDATE breakdown SET name = @Name, machine = @Machine, priority = @Priority, 
                start_time = @StartTime, end_time = @EndTime, description = @Description, 
                eliminated = @Eliminated WHERE id = @BreakdownId", new
            {
                Name = updatedBreakdown.Name,
                Machine = updatedBreakdown.Machine,
                Priority = updatedBreakdown.Priority,
                StartTime = updatedBreakdown.StartTime,
                EndTime = updatedBreakdown.EndTime,
                Description = updatedBreakdown.Description,
                Eliminated = updatedBreakdown.Eliminated,
                BreakdownId = breakdownId
            });

            return rowsAffected > 0;
        }

        public async Task<Breakdown> GetBreakdownById(Guid breakdownId) => await _connection.QueryFirstOrDefaultAsync<Breakdown>("SELECT * FROM breakdown WHERE id = @BreakdownId", new { BreakdownId = breakdownId });

        public async Task<bool> DeleteBreakdownById(Guid breakdownId)
        {
            if (await GetBreakdownById(breakdownId) != default(Breakdown))
            {
                var rowsAffected = await _connection.ExecuteAsync("DELETE FROM breakdown WHERE id = @BreakdownId", new { BreakdownId = breakdownId });

                return rowsAffected > 0;
            }

            return false;
        }

        public async Task<bool> UpdateBreakdownEliminationStatusById(Guid breakdownId, bool eliminated)
        {
            var rowsAffected = await _connection.ExecuteAsync(@"UPDATE breakdown SET eliminated = @Eliminated WHERE id = @BreakdownId", new
            {
                Eliminated = eliminated,
                BreakdownId = breakdownId
            });

            return rowsAffected > 0;
        }

        public async Task<bool> IsValidPriority(string breakdownPriority)
        {
            return breakdownPriority == "nizak" || breakdownPriority == "srednji" || breakdownPriority == "visok";
        }

        public async Task<List<Breakdown>> GetPaginatedSortedBreakdowns(int limit, int offset)
        {
            var query = @"SELECT * FROM breakdown
                          ORDER BY
                            CASE
                                WHEN priority = 'nizak' THEN 1
                                WHEN priority = 'srednji' THEN 2
                                WHEN priority = 'visok' THEN 3
                                ELSE 4
                            END, start_time DESC
                          LIMIT @Limit
                          OFFSET @Offset";

            var breakdowns = await _connection.QueryAsync<Breakdown>(query, new { Limit = limit, Offset = offset });

            return breakdowns.ToList();
        }
    }
}
