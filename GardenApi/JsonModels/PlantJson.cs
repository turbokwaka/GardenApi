namespace GardenApi;

public class PlantJson
{
    public List<PlantData> data { get; set; }
}
public class PlantData
{
    public string scientific_name { get; set; }
    public string common_name { get; set; }
    public string cycle { get; set; }
    public string watering_guide { get; set; }
    public string pruning_guide { get; set; }
    public string sunlight_guide { get; set; }
    public string image { get; set; }
}