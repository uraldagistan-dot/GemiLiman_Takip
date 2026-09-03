using PortTrackingSystem.Core.Entities;

namespace PortTrackingSystem.Core.Services
{
    public interface IShipVisitService
    {
        Task<IEnumerable<ShipVisits>> GetAllVisitsAsync();
        Task<ShipVisits?> GetVisitByIdAsync(int id);
        Task AddVisitAsync(ShipVisits visit);
        Task UpdateVisitAsync(int id, ShipVisits visit);
        Task DeleteVisitAsync(int id);
    }
}
