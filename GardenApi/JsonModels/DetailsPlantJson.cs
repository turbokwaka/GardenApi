namespace GardenApi;

public class DetailsPlantJson
{
    public int id { get; set; }
    public string common_name { get; set; }
    public List<string> scientific_name;
    public string cycle { get; set; }
    public DefaultImage default_image { get; set; }
}
