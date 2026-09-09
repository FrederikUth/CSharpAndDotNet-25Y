namespace Lektion03Opgave02;

// TODO: Opret den manglende 'LogHandler' delegate med den korrekte signatur her!

class Lektion03Opgave02Main
{
    static void Main(string[] args)
    {
        var processor = new LogProcessor();
        string[] systemLogs = { "Server startet", "Bruger logget ind", "Databaseforbindelse mistet" };

        // Koden sender et lambda-udtryk med to parametre
        processor.ProcessLogs(systemLogs, (message, timestamp) => 
        {
            Console.WriteLine($"[{timestamp:HH:mm:ss}] LOG: {message}");
        });
    }
}