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

        public async Task AddBreakdown(BreakdownRequest breakdown)
        {
            if (!await IsValidPriority(breakdown.Priority))
            {
                throw new ArgumentException("Invalid priority value");
            }

            if (string.IsNullOrWhiteSpace(breakdown.Description))
            {
                throw new ArgumentException("Description cannot be null or empty");
            }

            if (await DoesMachineHaveBreakdown(breakdown.Machine))
            {
                throw new InvalidOperationException("A breakdown is already associated with this machine");
            }

            var sql = "INSERT INTO breakdown (name, machine, priority, start_time, end_time, description, eliminated) " +
                      "VALUES (@Name, @Machine, @Priority, @StartTime, @EndTime, @Description, @Eliminated)";

            await _connection.ExecuteAsync(sql, breakdown);
        }

        public async Task<bool> UpdateBreakdownById(Guid breakdownId, BreakdownRequest updatedBreakdown)
        {
            if (!await IsValidPriority(updatedBreakdown.Priority))
            {
                throw new ArgumentException("Invalid priority value");
            }

            if (updatedBreakdown.Description == null || string.IsNullOrEmpty(updatedBreakdown.Description))
            {
                throw new ArgumentException("Description cannot be null or empty.");
            }

            if (await GetBreakdownById(breakdownId) == default(Breakdown))
            {
                return false;
            }

            if (await DoesMachineHaveBreakdown(updatedBreakdown.Machine))
            {
                if (await DoesMachineHaveBreakdown(updatedBreakdown.Machine))
                {
                    throw new InvalidOperationException("A breakdown is already associated with this machine.");
                }
            }

            var sql = @"UPDATE breakdown SET name = @Name, machine = @Machine, priority = @Priority, 
                       start_time = @StartTime, end_time = @EndTime, description = @Description, 
                       eliminated = @Eliminated WHERE id = @BreakdownId";

            var rowsAffected = await _connection.ExecuteAsync(sql, new
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
                return await _connection.ExecuteAsync("DELETE FROM breakdown WHERE id = @BreakdownId", new { BreakdownId = breakdownId }) > 0;
            }

            return false;
        }

        public async Task<bool> UpdateBreakdownEliminationStatusById(Guid breakdownId, bool eliminated) => await _connection.ExecuteAsync(@"UPDATE breakdown SET eliminated = @Eliminated WHERE id = @BreakdownId", new { Eliminated = eliminated, BreakdownId = breakdownId }) > 0;

        public async Task<bool> IsValidPriority(string breakdownPriority) => breakdownPriority == "nizak" || breakdownPriority == "srednji" || breakdownPriority == "visok";

        public async Task<IEnumerable<Breakdown>> GetPaginatedSortedBreakdowns(int limit, int offset)
        {
            var sql = @"SELECT * FROM breakdown
                          ORDER BY
                            CASE
                                WHEN priority = 'nizak' THEN 1
                                WHEN priority = 'srednji' THEN 2
                                WHEN priority = 'visok' THEN 3
                                ELSE 4
                            END, start_time DESC
                          LIMIT @Limit
                          OFFSET @Offset";

            return await _connection.QueryAsync<Breakdown>(sql, new { Limit = limit, Offset = offset });
        }

        public async Task<bool> DoesMachineHaveBreakdown(string machineName) => await _connection.QueryFirstOrDefaultAsync(@"SELECT COUNT(*) FROM breakdown WHERE machine = @MachineName", new { MachineName = machineName }) > 0;
    }
}
