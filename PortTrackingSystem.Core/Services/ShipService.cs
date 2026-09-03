using PortTrackingSystem.Core.Entities;
using PortTrackingSystem.Core.Repositories;

namespace PortTrackingSystem.Core.Services
{
    public class ShipService : IShipService //bağlantı
    {
        private readonly IRepository<Ship> _shipRepository;

        // Yazdığımız Repository'yi Dependency Injection (Bağımlılık Enjeksiyonu) ile içeri alıyoruz
        public ShipService(IRepository<Ship> shipRepository)
        {
            _shipRepository = shipRepository;
        }

        public async Task<IEnumerable<Ship>> GetAllShipsAsync()
        {
            return await _shipRepository.GetAllAsync();
        }

        public async Task<Ship?> GetShipByIdAsync(int id)
        {
            return await _shipRepository.GetByIdAsync(id);
        }

        public async Task AddShipAsync(Ship ship)
        {
            Validate(ship);

            // İŞ KURALI: IMO numarası benzersiz olmalı
            var allShips = await _shipRepository.GetAllAsync();
            bool imoExists = allShips.Any(s => s.IMO == ship.IMO);

            if (imoExists)
            {
                // Eğer aynı IMO numarası veritabanında varsa işlem durdurulur ve hata fırlatılır
                throw new Exception("Bu IMO numarasına sahip bir gemi zaten sistemde kayıtlı!");
            }

            // Kuraldan başarıyla geçtiyse veritabanına ekle
            await _shipRepository.AddAsync(ship);
        }

        public async Task UpdateShipAsync(int id, Ship ship)
        {
            var existing = await _shipRepository.GetByIdAsync(id);
            if (existing == null)
            {
                throw new Exception("Güncellenecek gemi bulunamadı!");
            }

            Validate(ship);

            // İŞ KURALI: IMO benzersizliği güncellemede de geçerli.
            // Geminin kendi kaydı hariç tutulur, aksi halde kendi IMO'suna takılır.
            var allShips = await _shipRepository.GetAllAsync();
            bool imoExists = allShips.Any(s => s.IMO == ship.IMO && s.ShipId != id);

            if (imoExists)
            {
                throw new Exception("Bu IMO numarasına sahip başka bir gemi zaten sistemde kayıtlı!");
            }

            existing.Name = ship.Name;
            existing.IMO = ship.IMO;
            existing.Type = ship.Type;
            existing.Flag = ship.Flag;
            existing.YearBuilt = ship.YearBuilt;

            _shipRepository.Update(existing);
        }

        public async Task DeleteShipAsync(int id)
        {
            var existing = await _shipRepository.GetByIdAsync(id);
            if (existing == null)
            {
                throw new Exception("Silinecek gemi bulunamadı!");
            }

            // İlişkili yük/ziyaret/atama kayıtları FK cascade ile birlikte silinir
            _shipRepository.Delete(existing);
        }

        public async Task<IEnumerable<Ship>> GetShipsWithDetailsAsync()
        {
            // CrewAssignments tek başına yeterli değil: mürettebatın adı/görevi
            // CrewMember üzerinde durduğu için o ilişkiyi de yüklüyoruz.
            return await _shipRepository.GetAllWithNestedIncludeAsync(
                "Cargoes",
                "CrewAssignments.CrewMember"
            );
        }

        // Ekleme ve güncellemede ortak kullanılan zorunlu alan kontrolleri
        private static void Validate(Ship ship)
        {
            if (string.IsNullOrWhiteSpace(ship.Name))
                throw new Exception("Gemi adı zorunludur!");

            if (string.IsNullOrWhiteSpace(ship.IMO))
                throw new Exception("IMO numarası zorunludur!");

            if (string.IsNullOrWhiteSpace(ship.Type))
                throw new Exception("Gemi tipi zorunludur!");

            if (string.IsNullOrWhiteSpace(ship.Flag))
                throw new Exception("Bayrak zorunludur!");

            if (ship.YearBuilt < 1900 || ship.YearBuilt > DateTime.Now.Year)
                throw new Exception($"Yapım yılı 1900 ile {DateTime.Now.Year} arasında olmalıdır!");
        }
    }
}
