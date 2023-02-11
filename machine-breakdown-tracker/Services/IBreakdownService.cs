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

    }
}
