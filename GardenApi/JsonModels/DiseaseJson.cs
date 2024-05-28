namespace GardenApi;

public class DiseasePlant
{
    public List<DiseasePlantData> data { get; set; }
}

public class DiseasePlantData
{
    public string common_name { get; set; }
    public string scientific_name { get; set; }
    public List<DiseaseDescription> description { get; set; }
    public List<DiseaseSolution> solution { get; set; }
    public List<DiseaseImage> images { get; set; }
}
public class DiseaseDescription
{
    public string subtitle { get; set; }
    public string description { get; set; }
}
public class DiseaseSolution
{
    public string subtitle { get; set; }
    public string description { get; set; }
}
public class DiseaseImage
{
    public string original_url { get; set; }
}