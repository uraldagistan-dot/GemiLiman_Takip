using Microsoft.AspNetCore.Mvc;
using PortTrackingSystem.Core.Entities;
using PortTrackingSystem.Core.Services;

namespace PortTrackingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CargoesController : ControllerBase
    {
        private readonly ICargoService _cargoService;

        public CargoesController(ICargoService cargoService)
        {
            _cargoService = cargoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cargoes = await _cargoService.GetAllCargoesAsync();
            return Ok(cargoes);
        }

        // EKRAN GEREKSİNİMİ: "Gemiye ait yüklerin listesi"
        [HttpGet("ship/{shipId}")]
        public async Task<IActionResult> GetByShipId(int shipId)
        {
            var cargoes = await _cargoService.GetCargoesByShipIdAsync(shipId);
            return Ok(cargoes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cargo = await _cargoService.GetCargoByIdAsync(id);
            if (cargo == null)
            {
                return NotFound("Yük bulunamadı!");
            }
            return Ok(cargo);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Cargoes cargo)
        {
            try
            {
                await _cargoService.AddCargoAsync(cargo);
                return Ok("Yük başarıyla eklendi!");
            }
            catch (Exception ex)
            {
                // Ağırlık 0 veya altındaysa yazdığımız o özel hata buraya düşecek
                return BadRequest(ex.Message); 
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Cargoes cargo)
        {
            try
            {
                await _cargoService.UpdateCargoAsync(id, cargo);
                return Ok("Yük başarıyla güncellendi!");
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
                await _cargoService.DeleteCargoAsync(id);
                return Ok("Yük başarıyla silindi!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
