using PortTrackingSystem.Core.Entities;
using PortTrackingSystem.Core.Repositories;

namespace PortTrackingSystem.Core.Services
{
    public class ShipVisitService : IShipVisitService
    {
        private readonly IRepository<ShipVisits> _visitRepository;
        private readonly IRepository<Ship> _shipRepository;
        private readonly IRepository<Port> _portRepository;

        public ShipVisitService(
            IRepository<ShipVisits> visitRepository,
            IRepository<Ship> shipRepository,
            IRepository<Port> portRepository)
        {
            _visitRepository = visitRepository;
            _shipRepository = shipRepository;
            _portRepository = portRepository;
        }

        public async Task<IEnumerable<ShipVisits>> GetAllVisitsAsync()
        {
            // Listede gemi ve liman adını gösterebilmek için ilişkileri de yüklüyoruz
            return await _visitRepository.GetAllWithNestedIncludeAsync("Ship", "Port");
        }

        public async Task<ShipVisits?> GetVisitByIdAsync(int id)
        {
            return await _visitRepository.GetByIdAsync(id);
        }

        public async Task AddVisitAsync(ShipVisits visit)
        {
            await ValidateAsync(visit);
            await _visitRepository.AddAsync(visit);
        }

        public async Task UpdateVisitAsync(int id, ShipVisits visit)
        {
            var existing = await _visitRepository.GetByIdAsync(id);
            if (existing == null)
            {
                throw new Exception("Güncellenecek ziyaret kaydı bulunamadı!");
            }

            await ValidateAsync(visit);

            existing.ShipId = visit.ShipId;
            existing.PortId = visit.PortId;
            existing.ArrivalDate = visit.ArrivalDate;
            existing.DepartureDate = visit.DepartureDate;
            existing.Purpose = visit.Purpose;

            _visitRepository.Update(existing);
        }

        public async Task DeleteVisitAsync(int id)
        {
            var existing = await _visitRepository.GetByIdAsync(id);
            if (existing == null)
            {
                throw new Exception("Silinecek ziyaret kaydı bulunamadı!");
            }

            _visitRepository.Delete(existing);
        }

        private async Task ValidateAsync(ShipVisits visit)
        {
            // İŞ KURALI 1: Seçilen gemi gerçekten var olmalı
            var ship = await _shipRepository.GetByIdAsync(visit.ShipId);
            if (ship == null)
            {
                throw new Exception("Seçilen gemi sistemde bulunamadı!");
            }

            // İŞ KURALI 2: Seçilen liman gerçekten var olmalı
            var port = await _portRepository.GetByIdAsync(visit.PortId);
            if (port == null)
            {
                throw new Exception("Seçilen liman sistemde bulunamadı!");
            }

            // İŞ KURALI 3: Ziyaret amacı zorunlu
            if (string.IsNullOrWhiteSpace(visit.Purpose))
            {
                throw new Exception("Ziyaret amacı zorunludur!");
            }

            // İŞ KURALI 4: Geliş tarihi, ayrılış tarihinden büyük veya eşit olamaz
            if (visit.ArrivalDate >= visit.DepartureDate)
            {
                throw new Exception("Hata: Gemi geliş tarihi, ayrılış tarihinden daha önce olmalıdır!");
            }
        }
    }
}
