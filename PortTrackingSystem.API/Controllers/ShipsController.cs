using Microsoft.AspNetCore.Mvc;
using PortTrackingSystem.Core.Entities;
using PortTrackingSystem.Core.Services;

namespace PortTrackingSystem.API.Controllers
{
    [ApiController] // Bu sınıfın bir API köprüsü olduğunu belirtir
    [Route("api/[controller]")] // Tarayıcıdaki adresi belirler (örn: localhost:xxxx/api/ships)
    public class ShipsController : ControllerBase
    {
        private readonly IShipService _shipService;

        // Dependency Injection ile yazdığımız servisi içeri alıyoruz
        public ShipsController(IShipService shipService)
        {
            _shipService = shipService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var ships = await _shipService.GetAllShipsAsync();
            return Ok(ships);
        }

        [HttpGet("details")] // Bu sayede adresi api/Ships/details olacak
        public async Task<IActionResult> GetShipsWithDetails()
        {
            var ships = await _shipService.GetShipsWithDetailsAsync();
            return Ok(ships);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ship = await _shipService.GetShipByIdAsync(id);
            if (ship == null)
            {
                return NotFound("Gemi bulunamadı!");
            }
            return Ok(ship);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Ship ship)
        {
            try
            {
                // Dışarıdan gelen gemi verisini servise (mutfağa) yolluyoruz
                await _shipService.AddShipAsync(ship);
                return Ok("Gemi başarıyla eklendi!");
            }
            catch (Exception ex)
            {
                // Eğer IMO numarası aynıysa, servisteki Exception fırlayacak ve buraya düşecek[cite: 1]
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Ship ship)
        {
            try
            {
                await _shipService.UpdateShipAsync(id, ship);
                return Ok("Gemi başarıyla güncellendi!");
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
                await _shipService.DeleteShipAsync(id);
                return Ok("Gemi başarıyla silindi!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
