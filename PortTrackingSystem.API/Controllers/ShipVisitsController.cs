using Microsoft.AspNetCore.Mvc;
using PortTrackingSystem.Core.Entities;
using PortTrackingSystem.Core.Services;

namespace PortTrackingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShipVisitsController : ControllerBase
    {
        private readonly IShipVisitService _visitService;

        public ShipVisitsController(IShipVisitService visitService)
        {
            _visitService = visitService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var visits = await _visitService.GetAllVisitsAsync();
            return Ok(visits);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var visit = await _visitService.GetVisitByIdAsync(id);
            if (visit == null)
            {
                return NotFound("Ziyaret kaydı bulunamadı!");
            }
            return Ok(visit);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ShipVisits visit)
        {
            try
            {
                await _visitService.AddVisitAsync(visit);
                return Ok("Gemi ziyareti başarıyla kaydedildi!");
            }
            catch (Exception ex)
            {
                // Tarih kuralı ihlal edilirse hata buraya düşecek
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ShipVisits visit)
        {
            try
            {
                await _visitService.UpdateVisitAsync(id, visit);
                return Ok("Ziyaret kaydı başarıyla güncellendi!");
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
                await _visitService.DeleteVisitAsync(id);
                return Ok("Ziyaret kaydı başarıyla silindi!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
