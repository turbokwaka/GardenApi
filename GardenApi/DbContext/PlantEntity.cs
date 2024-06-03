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
    public string scientific_name { get; set; }
    [Required]
    public string common_name { get; set; }
    [Required]
    public string cycle { get; set; }
    [Required]
    public string watering_guide { get; set; }
    [Required]
    public string pruning_guide { get; set; }
    [Required]
    public string sunlight_guide { get; set; }
    [Required]
    public string image { get; set; }
    [Required]
    public string lastWateringDate { get; set; }
}