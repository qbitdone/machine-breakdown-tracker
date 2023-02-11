using machine_breakdown_tracker.Models;

namespace machine_breakdown_tracker.Services
{
    public interface IBreakdownService
    {
        Task<IEnumerable<Breakdown>> GetAllBreakdowns();
        Task<Breakdown> GetBreakdownById(Guid breakdownId);
        Task<bool> AddBreakdown(BreakdownRequest breakdown);
        Task<bool> UpdateBreakdownById(Guid breakdownId, BreakdownRequest breakdown);
        Task<bool> DeleteBreakdownById(Guid breakdownId);
        Task<bool> UpdateBreakdownEliminationStatusById(Guid breakdownId, bool eliminated);
        Task<bool> IsValidPriority(string breakdownPriority);
        Task<IEnumerable<Breakdown>> GetPaginatedSortedBreakdowns(int limit, int offset);
    }
}
