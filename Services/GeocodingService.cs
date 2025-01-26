using System.Net.Http;
using Newtonsoft.Json.Linq;

public interface IGeocodingService
{
    Task<(string Country, string Region)> GetCountryAndRegion(double latitude, double longitude);
}


public class NominatimGeocodingService : IGeocodingService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public NominatimGeocodingService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<(string Country, string Region)> GetCountryAndRegion(double latitude, double longitude)
    {
        var client = _httpClientFactory.CreateClient("Nominatim");
        var response = await client.GetAsync($"reverse?format=json&lat={latitude}&lon={longitude}");
        
        if (!response.IsSuccessStatusCode) 
            return ("Unknown", "Unknown");

        var content = await response.Content.ReadAsStringAsync();
        var json = JObject.Parse(content);
        var address = json["address"];
        return (
            address?["country"]?.ToString() ?? "Unknown",
            address?["state"]?.ToString() ?? address?["county"]?.ToString() ?? "Unknown"
        );
    }
}