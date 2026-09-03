using System.ComponentModel.DataAnnotations; 
using System.ComponentModel.DataAnnotations.Schema; // ForeignKey için gerekli

namespace PortTrackingSystem.Core.Entities
{
    public class ShipCrewAssignments
    {   
        [Key]
        public int AssignmentId {get; set;}
        public int ShipId {get; set;}
        public int CrewId {get; set;}
        public DateTime AssignmentDate {get; set;}
        
        // Navigation property'ler nullable: aksi halde POST body'sinde zorunlu alan
        // sayılıp "The Ship field is required" doğrulama hatası veriyorlar.
        // İlişkinin zorunluluğunu ShipId/CrewId (int) alanları belirliyor.
        [ForeignKey("ShipId")]
        public Ship? Ship { get; set; }

        [ForeignKey("CrewId")]
        public CrewMembers? CrewMember { get; set; }
    }
}
