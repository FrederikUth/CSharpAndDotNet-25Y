namespace WeatherApi;

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public class WeatherService
{
    private readonly HttpClient _client = new();

    public async Task FetchAndPrintWeatherAsync(string city, CancellationToken cancellationToken)
    {
        string url = $"https://api.weather.com/v1/{city}";

        try
        {
            Console.WriteLine($"[INFO] Henter vejrdata for {city}...");
            
            // Send request med cancellation support
            HttpResponseMessage response = await _client.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync(cancellationToken);
            Console.WriteLine($"[SUCCESS] Data modtaget: {json.Substring(0, Math.Min(30, json.Length))}...");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"[CANCELLED] Anmodning for {city} blev afbrudt.");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[ERROR] HTTP-fejl for {city}: {ex.Message}");
        }
    }
}