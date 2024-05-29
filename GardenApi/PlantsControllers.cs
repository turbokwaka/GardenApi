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
        public async Task<IActionResult> SearchPlants([FromQuery] string name)
        {
            using (var client = new HttpClient())
            {
                // model to serialize
                var plantJson = new PlantJson { data = new List<PlantData>() };

                string apiUrl;

                apiUrl = $"https://perenual.com/api/species-list?key={_key}&q={name}";
                var speciesResponse = await client.GetStringAsync(apiUrl);
                var searchPlantData = JsonConvert.DeserializeObject<SearchPlant>(speciesResponse);

                foreach (var plant in searchPlantData.data.Take(2))
                {
                    if (plant.cycle.Contains("Upgrade Plans To Premium/Supreme"))
                        continue;

                    var guideUrl =
                        $"https://perenual.com/api/species-care-guide-list?key={_key}&species_id={plant.id}";
                    var guideResponse = await client.GetStringAsync(guideUrl);
                    var guidePlantData = JsonConvert.DeserializeObject<GuidePlant>(guideResponse);

                    plantJson.data.Add(new PlantData()
                    {
                        id = plant.id,
                        scientific_name = plant.scientific_name[0],
                        common_name = plant.common_name,
                        cycle = plant.cycle,
                        watering_guide = guidePlantData.data[0].section[0].description,
                        pruning_guide = guidePlantData.data[0].section[1].description,
                        sunlight_guide = guidePlantData.data[0].section[2].description,
                        image = plant.default_image != null
                            ? plant.default_image.original_url
                            : "https://postimg.cc/0rBn2kDn"
                    });
                    Console.WriteLine("done");
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