namespace asyncronExamples;

class AsyncronExample
{
    static async Task Main(string[] args)
    {
        // Starter begge tasks i baggrunden samtidigt
        Task<string> userTask = FetchUserAsync();
        Task<string> ordersTask = FetchOrdersAsync();

        Console.WriteLine(userTask.IsCompletedSuccessfully);
        // Venter på at BÅDE userTask og ordersTask er færdige (~2 sekunder totalt)
        await Task.WhenAll(userTask, ordersTask);

        string user = await userTask;
        string orders = await ordersTask;
        Console.WriteLine($"user: {user}, orders: {orders}");
        Console.WriteLine(userTask.IsCompletedSuccessfully);
    }

    private static async Task<string> FetchOrdersAsync()
    {
        await Task.Delay(1000);
        return "Some string";
    }

    private static async Task<string> FetchUserAsync()
    {
        await Task.Delay(2000);
        return "Another string";
    }
}