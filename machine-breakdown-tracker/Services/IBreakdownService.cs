using machine_breakdown_tracker.Models;

namespace machine_breakdown_tracker.Services
{
    public interface IBreakdownService
    {
        Task<IEnumerable<Breakdown>> GetAllBreakdowns();
        Task<bool> AddBreakdown(BreakdownRequest breakdown);
    }
}
