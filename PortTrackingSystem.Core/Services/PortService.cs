using PortTrackingSystem.Core.Entities;
using PortTrackingSystem.Core.Repositories;

namespace PortTrackingSystem.Core.Services
{
    public class PortService : IPortService
    {
        private readonly IRepository<Port> _portRepository;

        public PortService(IRepository<Port> portRepository)
        {
            _portRepository = portRepository;
        }

        public async Task<IEnumerable<Port>> GetAllPortsAsync()
        {
            return await _portRepository.GetAllAsync();
        }

        public async Task<Port?> GetPortByIdAsync(int id)
        {
            return await _portRepository.GetByIdAsync(id);
        }

        public async Task AddPortAsync(Port port)
        {
            Validate(port);

            // İŞ KURALI: Aynı şehirde aynı isimde iki liman olmasın
            var allPorts = await _portRepository.GetAllAsync();
            bool exists = allPorts.Any(p =>
                p.Name.ToLower() == port.Name.ToLower() &&
                p.City.ToLower() == port.City.ToLower());

            if (exists)
            {
                throw new Exception("Bu şehirde aynı isimde bir liman zaten kayıtlı!");
            }

            await _portRepository.AddAsync(port);
        }

        public async Task UpdatePortAsync(int id, Port port)
        {
            var existing = await _portRepository.GetByIdAsync(id);
            if (existing == null)
            {
                throw new Exception("Güncellenecek liman bulunamadı!");
            }

            Validate(port);

            var allPorts = await _portRepository.GetAllAsync();
            bool exists = allPorts.Any(p =>
                p.Name.ToLower() == port.Name.ToLower() &&
                p.City.ToLower() == port.City.ToLower() &&
                p.PortId != id);

            if (exists)
            {
                throw new Exception("Bu şehirde aynı isimde başka bir liman zaten kayıtlı!");
            }

            existing.Name = port.Name;
            existing.Country = port.Country;
            existing.City = port.City;

            _portRepository.Update(existing);
        }

        public async Task DeletePortAsync(int id)
        {
            var existing = await _portRepository.GetByIdAsync(id);
            if (existing == null)
            {
                throw new Exception("Silinecek liman bulunamadı!");
            }

            _portRepository.Delete(existing);
        }

        private static void Validate(Port port)
        {
            if (string.IsNullOrWhiteSpace(port.Name))
                throw new Exception("Liman adı zorunludur!");

            if (string.IsNullOrWhiteSpace(port.Country))
                throw new Exception("Ülke zorunludur!");

            if (string.IsNullOrWhiteSpace(port.City))
                throw new Exception("Şehir zorunludur!");
        }
    }
}
