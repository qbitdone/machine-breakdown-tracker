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
        public async Task<IActionResult> AddBreakdown([FromBody] BreakdownRequest breakdown)
        {
            try
            {
                await _breakdownService.AddBreakdown(breakdown);

                return Created("api/breakdown", breakdown);
            }

            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpPut("{breakdownId}")]
        public async Task<IActionResult> UpdateBreakdownById(Guid breakdownId, [FromBody] BreakdownRequest updatedBreakdown)
        {
            try
            {
                if (await _breakdownService.UpdateBreakdownById(breakdownId, updatedBreakdown))
                {
                    return NoContent();
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

            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{breakdownId}")]
        public async Task<IActionResult> DeleteBreakdownById(Guid breakdownId)
        {
            if (await _breakdownService.DeleteBreakdownById(breakdownId))
            {
                return NoContent();
            }
            return NotFound("Breakdown with provided id does not exist");
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

        [HttpGet("paginated")]
        public async Task<ActionResult<IEnumerable<Breakdown>>> GetBreakdowns(int limit, int offset) => Ok(await _breakdownService.GetPaginatedSortedBreakdowns(limit, offset));
    }
}
