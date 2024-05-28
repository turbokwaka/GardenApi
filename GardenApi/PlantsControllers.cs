using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GardenApi
{
    [ApiController]
    [Route("[controller]")]
    public class PlantsController : ControllerBase
    {
        private readonly string _key = "sk-YJuY66557a3fbe2fb5681";
        private readonly ApplicationContext _context;

        public PlantsController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet("plant/search")]
        public async Task<IActionResult> SearchPlants([FromQuery] string name = null, [FromQuery] string id = null)
        {
            using (var client = new HttpClient())
            {
                // model to serialize
                var plantJson = new PlantJson { data = new List<PlantData>() };

                string apiUrl;
        
                if (string.IsNullOrEmpty(name))
                {
                    apiUrl = $"https://perenual.com/api/species/details/{id}?key={_key}";
                    var speciesResponse = await client.GetStringAsync(apiUrl);
                    var searchPlantData = JsonConvert.DeserializeObject<DetailsPlantJson>(speciesResponse);

                    var guideUrl = $"https://perenual.com/api/species-care-guide-list?key={_key}&species_id={searchPlantData.id}";
                    var guideResponse = await client.GetStringAsync(guideUrl);
                    var guidePlantData = JsonConvert.DeserializeObject<GuidePlant>(guideResponse);
                    
                    plantJson.data.Add(new PlantData()
                    {
                        scientific_name = searchPlantData.scientific_name[0],
                        common_name = searchPlantData.common_name,
                        cycle = searchPlantData.cycle,
                        watering_guide = guidePlantData.data[0].section[0].description,
                        pruning_guide = guidePlantData.data[0].section[1].description,
                        sunlight_guide = guidePlantData.data[0].section[2].description,
                        image = searchPlantData.default_image != null ? searchPlantData.default_image.original_url : "https://postimg.cc/0rBn2kDn"
                    });
                    
                    Console.WriteLine("done");
                }
                else if (string.IsNullOrEmpty(id))
                {
                    apiUrl = $"https://perenual.com/api/pest-disease-list?key={_key}&species_id={name}";
                    var speciesResponse = await client.GetStringAsync(apiUrl);
                    var searchPlantData = JsonConvert.DeserializeObject<SearchPlant>(speciesResponse);

                    foreach (var plant in searchPlantData.data.Take(2))
                    {
                        if (plant.cycle.Contains("Upgrade Plans To Premium/Supreme"))
                            continue;

                        var guideUrl = $"https://perenual.com/api/species-care-guide-list?key={_key}&species_id={plant.id}";
                        var guideResponse = await client.GetStringAsync(guideUrl);
                        var guidePlantData = JsonConvert.DeserializeObject<GuidePlant>(guideResponse);

                        plantJson.data.Add(new PlantData()
                        {
                            scientific_name = plant.scientific_name[0],
                            common_name = plant.common_name,
                            cycle = plant.cycle,
                            watering_guide = guidePlantData.data[0].section[0].description,
                            pruning_guide = guidePlantData.data[0].section[1].description,
                            sunlight_guide = guidePlantData.data[0].section[2].description,
                            image = plant.default_image != null ? plant.default_image.original_url : "https://postimg.cc/0rBn2kDn"
                        });
                        Console.WriteLine("done");
                    }
                }
                else
                {
                    return BadRequest("Please provide either 'name' or 'speciesId'.");
                }
               

                var jsonResponse = JsonConvert.SerializeObject(plantJson);
                return new ContentResult
                {
                    Content = jsonResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };
            }
        }

        [HttpGet("plant/disease")]
        public async Task<IActionResult> SearchDisease([FromQuery] string name, [FromQuery] string id)
        {
            using (var client = new HttpClient())
            {
                var plantUrl = $"https://perenual.com/api/pest-disease-list?key={_key}&species_id={name}";
                var plantResponse = await client.GetStringAsync(plantUrl);
                var plantData = JsonConvert.DeserializeObject<DiseasePlant>(plantResponse);
                
                var jsonResponse = JsonConvert.SerializeObject(plantData);
                return new ContentResult
                {
                    Content = jsonResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };
            }   
        }
        
        [HttpPost("user/post")]
        public async Task<IActionResult> PostUser([FromBody] UserEntity user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok();
        }
        
        [HttpPost("plant/post")]
        public async Task<IActionResult> PostPlant([FromBody] PlantEntity plant)
        {
            if (plant == null)
            {
                return BadRequest();
            }
            
            _context.Plants.Add(plant);
            await _context.SaveChangesAsync();
            return Ok();
        }
        
        [HttpPut("plant/put")]
        public async Task<IActionResult> PutPlant([FromBody] PlantEntity plant)
        {
            if (plant == null)
            {
                return BadRequest("Plant data is null.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingPlant = await _context.Plants.FindAsync(plant.PlantId);
            if (existingPlant == null)
            {
                return NotFound($"Plant with ID {plant.PlantId} not found.");
            }

            _context.Entry(existingPlant).CurrentValues.SetValues(plant);
            await _context.SaveChangesAsync();
            return Ok();
        }
        
        [HttpDelete("plant/delete")]
        public async Task<IActionResult> DeletePlant([FromBody] PlantEntity plant)
        {
            if (plant == null)
            {
                return BadRequest();
            }
            
            _context.Plants.Remove(plant);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
