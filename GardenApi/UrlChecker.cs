namespace GardenApi;

public static class UrlChecker
{
    private static readonly HttpClient httpClient = new HttpClient();

    public static async Task<bool> IsUrlValid(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            throw new ArgumentException("URL cannot be null or empty.", nameof(url));
        }

        try
        {
            var response = await httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            // Check for specific image URL in the response content
            if (content.Contains("https://perenual.com/storage/image/error_asset/plant.png"))
            {
                return false; // The page contains the error image, so it's a 404 page
            }

            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }
        catch (HttpRequestException)
        {
            return false; // URL is invalid or there was a request error
        }
    }
}