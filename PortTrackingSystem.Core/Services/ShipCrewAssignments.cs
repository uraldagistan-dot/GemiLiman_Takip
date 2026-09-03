using PortTrackingSystem.Core.Entities;
using PortTrackingSystem.Core.Repositories;

namespace PortTrackingSystem.Core.Services
{
    public class ShipCrewAssignmentService : IShipCrewAssignmentService
    {
        private readonly IRepository<ShipCrewAssignments> _assignmentRepository;
        private readonly IRepository<Ship> _shipRepository;
        private readonly IRepository<CrewMembers> _crewRepository;

        public ShipCrewAssignmentService(
            IRepository<ShipCrewAssignments> assignmentRepository,
            IRepository<Ship> shipRepository,
            IRepository<CrewMembers> crewRepository)
        {
            _assignmentRepository = assignmentRepository;
            _shipRepository = shipRepository;
            _crewRepository = crewRepository;
        }

        public async Task<IEnumerable<ShipCrewAssignments>> GetAllAssignmentsAsync()
        {
            return await _assignmentRepository.GetAllWithNestedIncludeAsync("Ship", "CrewMember");
        }

        public async Task AddAssignmentAsync(ShipCrewAssignments assignment)
        {
            // İŞ KURALI 1: Gemi gerçekten var olmalı
            var ship = await _shipRepository.GetByIdAsync(assignment.ShipId);
            if (ship == null)
            {
                throw new Exception("Seçilen gemi sistemde bulunamadı!");
            }

            // İŞ KURALI 2: Mürettebat gerçekten var olmalı
            var crew = await _crewRepository.GetByIdAsync(assignment.CrewId);
            if (crew == null)
            {
                throw new Exception("Seçilen mürettebat sistemde bulunamadı!");
            }

            // Tarih gönderilmediyse bugünü kullan
            if (assignment.AssignmentDate == default)
            {
                assignment.AssignmentDate = DateTime.Now;
            }

            // İŞ KURALI 3: Aynı mürettebat aynı gemide AYNI TARİHTE birden fazla kez atanamaz.
            // Saat farkı önemli değil, sadece gün karşılaştırılır.
            var allAssignments = await _assignmentRepository.GetAllAsync();
            bool alreadyAssigned = allAssignments.Any(a =>
                a.ShipId == assignment.ShipId &&
                a.CrewId == assignment.CrewId &&
                a.AssignmentDate.Date == assignment.AssignmentDate.Date);

            if (alreadyAssigned)
            {
                throw new Exception("Bu personel bu gemiye aynı tarihte zaten atanmış!");
            }

            await _assignmentRepository.AddAsync(assignment);
        }

        public async Task DeleteAssignmentAsync(int id)
        {
            var existing = await _assignmentRepository.GetByIdAsync(id);
            if (existing == null)
            {
                throw new Exception("Silinecek atama kaydı bulunamadı!");
            }

            _assignmentRepository.Delete(existing);
        }
    }
}
