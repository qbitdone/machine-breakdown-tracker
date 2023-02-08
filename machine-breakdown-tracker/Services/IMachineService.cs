using machine_breakdown_tracker.Models;

namespace machine_breakdown_tracker.Services
{
    public interface IMachineService
    {
        Task<IEnumerable<Machine>> GetAllMachines();
        Task<bool> AddMachine(Machine machine);
        Task<bool> DoesMachineNameExist(Machine machine);
        Task<bool> DeleteMachineByName(string machineName);
    }
}
