using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace flic2lib.Maui.Sample.Services;

public interface IIncidentService
{
    Task<bool> RaiseIncidentAsync(string buttonId, string incidentType = "Emergency");
}

public class IncidentService : IIncidentService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiBaseUrl;

    public IncidentService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiBaseUrl = configuration["IncidentApi:BaseUrl"] ?? "https://your-api-endpoint.com";
    }

    public async Task<bool> RaiseIncidentAsync(string buttonId, string incidentType = "Emergency")
    {
        try
        {
            var incident = new
            {
                ButtonId = buttonId,
                IncidentType = incidentType,
                Timestamp = DateTime.UtcNow,
                Location = await GetLocationAsync(), // Optional: add location
                DeviceInfo = GetDeviceInfo()
            };

            var json = JsonSerializer.Serialize(incident);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/incidents", content);

            if (response.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine($"Incident raised successfully for button {buttonId}");
                return true;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Failed to raise incident: {response.StatusCode}");
                return false;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error raising incident: {ex.Message}");
            return false;
        }
    }

    private async Task<object?> GetLocationAsync()
    {
        try
        {
            // Add location services if needed
            // var location = await Geolocation.GetLocationAsync();
            // return location != null ? new { Latitude = location.Latitude, Longitude = location.Longitude } : null;
            return null;
        }
        catch
        {
            return null;
        }
    }

    private object GetDeviceInfo()
    {
        return new
        {
            Platform = DeviceInfo.Platform.ToString(),
            Model = DeviceInfo.Model,
            Version = DeviceInfo.VersionString
        };
    }
}