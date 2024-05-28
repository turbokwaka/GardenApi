using System.ComponentModel.DataAnnotations;

namespace GardenApi;

public class UserEntity
{
    [Key]
    public int UserId { get; set; }
    [Required]
    public string Name { get; set; }
    
    public ICollection<PlantEntity> Plants { get; set; }
}