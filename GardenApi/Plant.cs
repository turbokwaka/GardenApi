namespace GardenApi;

public class Datum
{
    public string common_name;
    public List<string> scientific_name;
    public List<string?> other_name;
    public string cycle;
    public string watering;
    public object sunlight;
    public DefaultImage default_image;
}

public class DefaultImage
{
    public string original_url;
}

public class Root
{
    public List<Datum> data;
}