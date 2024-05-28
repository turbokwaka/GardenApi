using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GardenApi;

public class PlantEntity
{
    [Key] 
    public int PlantId { get; set; }

    // one to many relationship
    [ForeignKey("UserId")]
    public int? UserId { get; set; }
    // actual data
    [Required]
    public DateTime wateringFrequency { get; set; }
    [Required]
    public DateTime pruningFrequency { get; set; }
    [Required]
    public DateTime lastWateringDate { get; set; }
    [Required]
    public DateTime lastPruningDate { get; set; }
}