using PortTrackingSystem.Core.Entities;

namespace PortTrackingSystem.Core.Services
{
    public interface ICargoService
    {
        Task<IEnumerable<Cargoes>> GetAllCargoesAsync();
        Task<IEnumerable<Cargoes>> GetCargoesByShipIdAsync(int shipId);
        Task<Cargoes?> GetCargoByIdAsync(int id);
        Task AddCargoAsync(Cargoes cargo);
        Task UpdateCargoAsync(int id, Cargoes cargo);
        Task DeleteCargoAsync(int id);
    }
}
