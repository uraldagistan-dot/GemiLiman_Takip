using System.ComponentModel.DataAnnotations; 
namespace PortTrackingSystem.Core.Entities
{
    public class Ship
    {   [Key]
        public int ShipId { get; set; } 
        public string Name { get; set; } = string.Empty;
        public string IMO { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Flag { get; set; } = string.Empty;
        public int YearBuilt { get; set; }
        // Bir geminin birden fazla yükü olabilir 
        public ICollection<Cargoes> Cargoes { get; set; } = new List<Cargoes>();
        public ICollection<ShipVisits> ShipVisits { get; set; } = new List<ShipVisits>();
        public ICollection<ShipCrewAssignments> CrewAssignments { get; set; } = new List<ShipCrewAssignments>();
    }
}
