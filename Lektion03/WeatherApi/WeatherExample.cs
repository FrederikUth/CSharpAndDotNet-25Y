namespace WeatherApi;

public class WeatherExample
{
    public static async Task Main()
    {
        var service = new WeatherService();
        using var cts = new CancellationTokenSource();
        
        // Sæt en samlet timeout på 2 sekunder
        cts.CancelAfter(TimeSpan.FromSeconds(2));

        // Kør 2 byer i parallel
        Task task1 = service.FetchAndPrintWeatherAsync("Copenhagen", cts.Token);
        Task task2 = service.FetchAndPrintWeatherAsync("Aarhus", cts.Token);

        await Task.WhenAll(task1, task2);
        
        Console.WriteLine("Udførelse gennemført.");
    }
}