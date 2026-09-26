using System.ComponentModel.DataAnnotations; 

namespace PortTrackingSystem.Core.Entities
{
    public class CrewMembers
    {
        [Key]
        public int CrewId {get; set;}
        public string FirstName {get; set;} = string.Empty;
        public string LastName {get; set;} = string.Empty;
        // Formda zorunlu değil
        public string? Email {get; set;}
        public string? PhoneNumber {get; set;}
        public string Role {get; set;} = string.Empty;
        public ICollection<ShipCrewAssignments> CrewAssignments { get; set; } = new List<ShipCrewAssignments>();

    }

}
