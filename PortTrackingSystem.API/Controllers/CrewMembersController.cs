using Microsoft.AspNetCore.Mvc;
using PortTrackingSystem.Core.Entities;
using PortTrackingSystem.Core.Services;

namespace PortTrackingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrewMembersController : ControllerBase
    {
        // Repository yerine Service katmanı kullanılıyor: e-posta/telefon/görev
        // validasyonları iş kuralı olduğu için servise ait.
        private readonly ICrewMemberService _crewService;

        public CrewMembersController(ICrewMemberService crewService)
        {
            _crewService = crewService;
        }

        // GET: api/CrewMembers
        [HttpGet]
        public async Task<IActionResult> GetAllCrewMembers()
        {
            var crew = await _crewService.GetAllCrewMembersAsync();
            return Ok(crew);
        }

        // GET: api/CrewMembers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var crew = await _crewService.GetCrewMemberByIdAsync(id);
            if (crew == null)
            {
                return NotFound("Personel bulunamadı!");
            }
            return Ok(crew);
        }

        // POST: api/CrewMembers
        [HttpPost]
        public async Task<IActionResult> AddCrewMember([FromBody] CrewMembers newCrew)
        {
            if (newCrew == null)
            {
                return BadRequest("Gönderilen veri boş olamaz.");
            }

            try
            {
                await _crewService.AddCrewMemberAsync(newCrew);
                return Ok(newCrew);
            }
            catch (Exception ex)
            {
                // Validasyon hataları (e-posta formatı, zorunlu görev vb.) buraya düşer
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/CrewMembers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CrewMembers crew)
        {
            try
            {
                await _crewService.UpdateCrewMemberAsync(id, crew);
                return Ok("Personel başarıyla güncellendi!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/CrewMembers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _crewService.DeleteCrewMemberAsync(id);
                return Ok("Personel başarıyla silindi!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
