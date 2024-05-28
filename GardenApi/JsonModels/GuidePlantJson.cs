namespace GardenApi;

public class GuidePlant
{
    public List<GuidePlantData> data { get; set; }
}
public class GuidePlantData
{
    public List<Section> section { get; set; }
}

public class Section
{
    public string type { get; set; }
    public string description { get; set; }
}

