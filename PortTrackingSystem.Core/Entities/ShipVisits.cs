using System.ComponentModel.DataAnnotations; 
using System.ComponentModel.DataAnnotations.Schema;

namespace PortTrackingSystem.Core.Entities
{
    public class ShipVisits
    {   [Key]
        public int VisitId {get; set;}
        public int ShipId {get;set;}
        public int PortId {get;set;}
        public DateTime ArrivalDate {get;set;}
        public DateTime DepartureDate {get;set;}
        public string Purpose {get; set;} = string.Empty;

        // Ziyaret listesinde gemi ve liman adını gösterebilmek için.
        // Nullable: POST body'sinde zorunlu alan sayılmamaları gerekiyor.
        [ForeignKey("ShipId")]
        public Ship? Ship { get; set; }

        [ForeignKey("PortId")]
        public Port? Port { get; set; }
    }

}
