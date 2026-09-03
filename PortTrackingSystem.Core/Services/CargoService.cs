using PortTrackingSystem.Core.Entities;
using PortTrackingSystem.Core.Repositories;

namespace PortTrackingSystem.Core.Services
{
    public class CargoService : ICargoService
    {
        private readonly IRepository<Cargoes> _cargoRepository;
        private readonly IRepository<Ship> _shipRepository;

        public CargoService(IRepository<Cargoes> cargoRepository, IRepository<Ship> shipRepository)
        {
            _cargoRepository = cargoRepository;
            _shipRepository = shipRepository;
        }

        public async Task<IEnumerable<Cargoes>> GetAllCargoesAsync()
        {
            // Listede geminin adını gösterebilmek için Ship ilişkisini de yüklüyoruz
            return await _cargoRepository.GetAllWithIncludeAsync(c => c.Ship!);
        }

        // "Gemiye ait yüklerin listesi" ekran gereksinimi için
        public async Task<IEnumerable<Cargoes>> GetCargoesByShipIdAsync(int shipId)
        {
            var all = await _cargoRepository.GetAllWithIncludeAsync(c => c.Ship!);
            return all.Where(c => c.ShipId == shipId);
        }

        public async Task<Cargoes?> GetCargoByIdAsync(int id)
        {
            return await _cargoRepository.GetByIdAsync(id);
        }

        public async Task AddCargoAsync(Cargoes cargo)
        {
            await ValidateAsync(cargo);
            await _cargoRepository.AddAsync(cargo);
        }

        public async Task UpdateCargoAsync(int id, Cargoes cargo)
        {
            var existing = await _cargoRepository.GetByIdAsync(id);
            if (existing == null)
            {
                throw new Exception("Güncellenecek yük bulunamadı!");
            }

            await ValidateAsync(cargo);

            existing.Description = cargo.Description;
            existing.WeightTon = cargo.WeightTon;
            existing.CargoType = cargo.CargoType;
            existing.ShipId = cargo.ShipId;

            _cargoRepository.Update(existing);
        }

        public async Task DeleteCargoAsync(int id)
        {
            var existing = await _cargoRepository.GetByIdAsync(id);
            if (existing == null)
            {
                throw new Exception("Silinecek yük bulunamadı!");
            }

            _cargoRepository.Delete(existing);
        }

        private async Task ValidateAsync(Cargoes cargo)
        {
            if (string.IsNullOrWhiteSpace(cargo.Description))
                throw new Exception("Yük açıklaması zorunludur!");

            if (string.IsNullOrWhiteSpace(cargo.CargoType))
                throw new Exception("Yük tipi zorunludur!");

            // Yük ağırlığı 0 veya daha küçük olamaz
            if (cargo.WeightTon <= 0)
            {
                throw new Exception("Hata: Yük ağırlığı (WeightTon) 0'dan büyük olmalıdır!");
            }

            //Yük mutlaka var olan bir gemiye ait olmalı
            var ship = await _shipRepository.GetByIdAsync(cargo.ShipId);
            if (ship == null)
            {
                throw new Exception("Seçilen gemi sistemde bulunamadı!");
            }
        }
    }
}
