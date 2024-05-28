using Newtonsoft.Json.Linq;

namespace GardenApi;
public class SearchPlant
{
    public List<SearchPlantData> data;
}

public class SearchPlantData
{
    public int id;
    public string common_name;
    public List<string> scientific_name;
    public string cycle;
    public DefaultImage default_image;
}

public class DefaultImage
{
    public string original_url;
}