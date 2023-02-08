using machine_breakdown_tracker.Models;

namespace machine_breakdown_tracker.Services
{
    public interface IMachineService
    {
        Task<IEnumerable<Machine>> GetAllMachines();
    }
}
