using machine_breakdown_tracker.Models;
using machine_breakdown_tracker.Services;
using Microsoft.AspNetCore.Mvc;

namespace machine_breakdown_tracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MachineController : ControllerBase
    {
        private readonly IMachineService _machineService;
        public MachineController(IMachineService machineService)
        {
            _machineService = machineService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Machine>>> GetAllMachines() => Ok(await _machineService.GetAllMachines());
    }
}
