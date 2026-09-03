using PortTrackingSystem.Core.Entities;

namespace PortTrackingSystem.Core.Services
{
    public interface IShipService
    {
        Task<IEnumerable<Ship>> GetAllShipsAsync();
        Task<Ship?> GetShipByIdAsync(int id);
        Task AddShipAsync(Ship ship);
        Task UpdateShipAsync(int id, Ship ship);
        Task DeleteShipAsync(int id);
        Task<IEnumerable<Ship>> GetShipsWithDetailsAsync();
    }
}
