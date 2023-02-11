using machine_breakdown_tracker.Models;
using machine_breakdown_tracker.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace machine_breakdown_tracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BreakdownController : ControllerBase
    {
        private readonly IBreakdownService _breakdownService;
        public BreakdownController(IBreakdownService breakdownService)
        {
            _breakdownService = breakdownService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Machine>>> GetAllMachines() => Ok(await _breakdownService.GetAllBreakdowns());

        [HttpPost]
        public async Task<ActionResult<bool>> AddBreakdown([FromBody] BreakdownRequest breakdown)
        {
            if (await _breakdownService.AddBreakdown(breakdown))
            {
                return Ok("You have successfully added new breakdown!");
            }
            else
            {
                return BadRequest("Failed to add breakdown. One or more required fields are missing or have invalid values.");
            }
        }

        [HttpPut("{breakdownId}")]
        public async Task<IActionResult> UpdateBreakdownById(Guid breakdownId, [FromBody] BreakdownRequest updatedBreakdown)
        {
            try
            {
                if (await _breakdownService.UpdateBreakdownById(breakdownId, updatedBreakdown))
                {
                    return Ok("Breakdown updated successfully");
                }

                else
                {
                    return NotFound("No breakdown found with the provided ID");
                }
            }

            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{breakdownId}")]
        public async Task<ActionResult> DeleteBreakdownById(Guid breakdownId)
        {
            if (!await _breakdownService.DeleteBreakdownById(breakdownId))
            {
                return NotFound("Breakdown with provided id does not exist");
            }
            return Ok($"You have successfully deleted breakdown with id: {breakdownId}");
        }

        [HttpPut("{breakdownId}/status")]
        public async Task<IActionResult> UpdateBreakdownEliminationStatus(Guid breakdownId, [FromBody] bool eliminated)
        {
            if (await _breakdownService.UpdateBreakdownEliminationStatusById(breakdownId, eliminated))
            {
                return Ok("You have sucesfully updated eliminated status of breakdown");
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
