using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GardenApi
{
    [ApiController]
    [Route("[controller]")]
    public class PlantsController : ControllerBase
    {
        private readonly string _key = "sk-IezH6630ee7f8f86e5294";
        
        [HttpGet("plant/search")]
        public async Task<IActionResult> GetPlantability([FromQuery] string keyword)
        {
            using (HttpClient client = new HttpClient())
            {
                var url = $"https://perenual.com/api/species-list?key={_key}&q={keyword}";
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                
                string json = await response.Content.ReadAsStringAsync();
                Root plantData = JsonConvert.DeserializeObject<Root>(json);
                
                string jsonResponse = JsonConvert.SerializeObject(plantData);

                // Create an HTTP content with JSON data
                var content = new ContentResult
                {
                    Content = jsonResponse,
                    ContentType = "application/json",
                    StatusCode = 200 // OK status code
                };

                return content;
            }
        }
    }
}