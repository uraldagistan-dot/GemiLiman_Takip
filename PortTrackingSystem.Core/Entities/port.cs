using System.ComponentModel.DataAnnotations; 

namespace PortTrackingSystem.Core.Entities
{
    public class Port
    {   [Key]
        public int PortId {get; set;}
        public string Name {get;set;} = string.Empty;
        public string Country {get;set;} = string.Empty;
        public string City {get;set;} = string.Empty;
        public ICollection<ShipVisits> ShipVisits { get; set; } = new List<ShipVisits>();
    }



}
