using System.ComponentModel.DataAnnotations; 
using System.ComponentModel.DataAnnotations.Schema; 
namespace PortTrackingSystem.Core.Entities
{
    public class Cargoes
    {
        [Key]
        public int CargoId {get; set;}
        public int ShipId {get;set;}
        public Ship? Ship { get; set; } // Hangi gemiye ait olduğunu belirtir
        public string Description {get; set;} = string.Empty;

        [Column(TypeName = "decimal(10,2)")] // Dokümanda istenen tam SQL tipi
        public decimal WeightTon {get;set;}  
        public string CargoType {get; set;} = string.Empty;


    }

}
