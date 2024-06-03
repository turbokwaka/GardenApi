using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace GardenApi
{
    [ApiController]
    [Route("[controller]")]
    public class PlantsController : ControllerBase
    {
        private readonly string _key = "sk-8i2U66567185637a85692";
        private readonly ApplicationContext _context;

        public PlantsController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet("plant/search")]
        public async Task<IActionResult> SearchPlants([FromQuery] string? name = null, [FromQuery] string? plantId = null)
        {
            var client = new HttpClient();
            var plantJson = new PlantJson { data = new List<PlantData>() };
            string url;

            if (name != null)
            {
                Console.WriteLine("\nStarted plants searching by name: " + name);
                url = $"https://perenual.com/api/species-list?key={_key}&q={name}";
                var searchPlantResponse = await client.GetStringAsync(url);
                var searchPlantData = JsonConvert.DeserializeObject<SearchPlant>(searchPlantResponse);

                foreach (var plant in searchPlantData!.data.Take(5))
                {
                    if (plant.cycle.Contains("Upgrade Plans To Premium/Supreme"))
                        continue;

                    var guideUrl =
                        $"https://perenual.com/api/species-care-guide-list?key={_key}&species_id={plant.id}";
                    var guidePlantResponse = await client.GetStringAsync(guideUrl);
                    var guidePlantData = JsonConvert.DeserializeObject<GuidePlant>(guidePlantResponse);
                    
                    bool imageIsValid = await UrlChecker.IsUrlValid(plant.default_image.original_url) ? true : false;
                    
                    plantJson.data.Add(new PlantData()
                    {
                        plantId = plant.id,
                        scientific_name = plant.scientific_name[0],
                        common_name = plant.common_name,
                        cycle = plant.cycle,
                        watering_guide = guidePlantData.data[0].section[0].description,
                        pruning_guide = guidePlantData.data[0].section[2].description,
                        sunlight_guide = guidePlantData.data[0].section[1].description,
                        image = imageIsValid
                            ? plant.default_image.original_url 
                            : "https://postimg.cc/0rBn2kDn"
                    });
                    Console.WriteLine("done");
                }
            }
            if (plantId != null)
            {
                Console.WriteLine("\nStarted plants searching by id: " + plantId);
                url = $"https://perenual.com/api/species/details/{plantId}?key={_key}";
                var detailsPlantResponse = await client.GetStringAsync(url);
                var detailsPlantData = JsonConvert.DeserializeObject<SearchPlantData>(detailsPlantResponse);

                url = $"https://perenual.com/api/species-care-guide-list?key={_key}&species_id={plantId}";
                var guidePlantResponse = await client.GetStringAsync(url);
                var guidePlantData = JsonConvert.DeserializeObject<GuidePlant>(guidePlantResponse);
                
                bool imageIsValid = await UrlChecker.IsUrlValid(detailsPlantData.default_image.original_url) ? true : false;
                
                plantJson.data.Add(new PlantData()
                {
                    plantId = int.Parse(plantId),
                    scientific_name = detailsPlantData.scientific_name[0],
                    common_name = detailsPlantData.common_name,
                    cycle = detailsPlantData.cycle,
                    watering_guide = guidePlantData.data[0].section[0].description,
                    pruning_guide = guidePlantData.data[0].section[2].description,
                    sunlight_guide = guidePlantData.data[0].section[1].description,
                    image = imageIsValid
                        ? detailsPlantData.default_image.original_url
                        : "https://postimg.cc/0rBn2kDn"
                });
            }

            if (plantJson.data.Count == 0)
            {
                return NotFound("No plants found.");
            }
            
            var jsonResponse = JsonConvert.SerializeObject(plantJson);
            return new ContentResult
            {
                Content = jsonResponse,
                ContentType = "application/json",
                StatusCode = 200
            };
        }

        [HttpGet("plant/user-plants-list")]
        public async Task<IActionResult> SearchUserPlants([FromQuery] string userId)
        {
            // Check if the user already exists in the database
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.UserId == int.Parse(userId));
            if (existingUser == null)
            {
                return NotFound("User not found.");
            }

            var plants = await _context.Plants
                .Where(p => p.UserId == int.Parse(userId))
                .ToListAsync();
            
            var jsonResponse = JsonConvert.SerializeObject(plants);
            return new ContentResult
            {
                Content = jsonResponse,
                ContentType = "application/json",
                StatusCode = 200
            };
        }

        [HttpPost("user/post")]
        public async Task<IActionResult> PostUser([FromBody] UserEntity user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            // Check if the user already exists in the database
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.UserId == user.UserId); // Adjust the condition as per your unique fields

            if (existingUser != null)
            {
                return Conflict("User already exists.");
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
        public async Task<IActionResult> DeletePlant(int userId, int plantId)
        {
            var plant = _context.Plants.FirstOrDefault(p => p.UserId == userId && p.PlantId == plantId);
            if (plant == null)
            {
                return NotFound();
            }

            _context.Plants.Remove(plant);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}