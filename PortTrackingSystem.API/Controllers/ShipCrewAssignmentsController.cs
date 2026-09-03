using Microsoft.AspNetCore.Mvc;
using PortTrackingSystem.Core.Entities;
using PortTrackingSystem.Core.Services;

namespace PortTrackingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShipCrewAssignmentsController : ControllerBase
    {
        private readonly IShipCrewAssignmentService _assignmentService;

        public ShipCrewAssignmentsController(IShipCrewAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var assignments = await _assignmentService.GetAllAssignmentsAsync();
            return Ok(assignments);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ShipCrewAssignments assignment)
        {
            try
            {
                await _assignmentService.AddAssignmentAsync(assignment);
                return Ok("Mürettebat gemiye başarıyla atandı!");
            }
            catch (Exception ex)
            {
                // İş kuralı ihlallerini (olmayan gemi/personel, aynı tarihte mükerrer atama) anlamlı mesajla döner
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _assignmentService.DeleteAssignmentAsync(id);
                return Ok("Atama başarıyla kaldırıldı!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
