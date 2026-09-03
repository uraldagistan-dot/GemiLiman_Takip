using PortTrackingSystem.Core.Entities;

namespace PortTrackingSystem.Core.Services
{
    public interface IShipCrewAssignmentService
    {
        Task<IEnumerable<ShipCrewAssignments>> GetAllAssignmentsAsync();
        Task AddAssignmentAsync(ShipCrewAssignments assignment);
        Task DeleteAssignmentAsync(int id);
    }
}
