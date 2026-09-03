using PortTrackingSystem.Core.Entities;
namespace PortTrackingSystem.Core.Services
{
    public interface IPortService
    {
        Task<IEnumerable<Port>> GetAllPortsAsync();
        Task<Port?> GetPortByIdAsync(int id);
        Task AddPortAsync(Port port);
        Task UpdatePortAsync(int id, Port port);
        Task DeletePortAsync(int id);
    }
}
