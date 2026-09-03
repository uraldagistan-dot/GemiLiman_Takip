using Microsoft.AspNetCore.Mvc;
using PortTrackingSystem.Core.Entities;
using PortTrackingSystem.Core.Services;

namespace PortTrackingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PortsController : ControllerBase
    {
        private readonly IPortService _portService;

        public PortsController(IPortService portService)
        {
            _portService = portService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var ports = await _portService.GetAllPortsAsync();
            return Ok(ports);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var port = await _portService.GetPortByIdAsync(id);
            if (port == null)
            {
                return NotFound("Liman bulunamadı!");
            }
            return Ok(port);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Port port)
        {
            try
            {
                await _portService.AddPortAsync(port);
                return Ok("Liman başarıyla eklendi!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Port port)
        {
            try
            {
                await _portService.UpdatePortAsync(id, port);
                return Ok("Liman başarıyla güncellendi!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _portService.DeletePortAsync(id);
                return Ok("Liman başarıyla silindi!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
