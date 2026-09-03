using PortTrackingSystem.Core.Entities;

namespace PortTrackingSystem.Core.Services
{
    public interface ICrewMemberService
    {
        Task<IEnumerable<CrewMembers>> GetAllCrewMembersAsync();
        Task<CrewMembers?> GetCrewMemberByIdAsync(int id);
        Task AddCrewMemberAsync(CrewMembers crewMember);
        Task UpdateCrewMemberAsync(int id, CrewMembers crewMember);
        Task DeleteCrewMemberAsync(int id);
    }
}
