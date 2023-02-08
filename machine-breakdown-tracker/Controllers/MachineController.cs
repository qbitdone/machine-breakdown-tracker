using machine_breakdown_tracker.Models;
using machine_breakdown_tracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Xml.Linq;
using Machine = machine_breakdown_tracker.Models.Machine;

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

        [HttpPost]
        public async Task<ActionResult> AddMachine([FromBody] Machine machine)
        {
            if (!await _machineService.DoesMachineNameExist(machine))
            {
                return BadRequest("A Machine with that name already exists");
            }
            if (await _machineService.AddMachine(machine))
            {
                return Ok("You have successfully added new Machine!");
            }
            else
            {
                return BadRequest("Invalid machine name");
            }
        }

        [HttpDelete("{machineName}")]
        public async Task<ActionResult> DeleteMovieById(string machineName)
        {
            if (!await _machineService.DeleteMachineByName(machineName))
            {
                return NotFound($"Machine with name: {machineName} does not exists");
            }
            return Ok($"You have successfully deleted machine with name: {machineName}");
        }
    }
}
