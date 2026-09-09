# Opgaver: Lektion 03

### Opgave 1: Custom Delegate – Simpel Lommeregner

**Fokus:** Erklæring af `delegate`, tildeling af metoder og lambda-udtryk.

**Beskrivelse:**  
I denne opgave skal du oprette din egen custom `delegate`-type til at udføre matematiske beregninger på to `double`-værdier. Opgaven skal løses i projektet `Lektion03Opgave01` i filen `Lektion03Opgave01Main.cs`.

1. Deklarer en delegate-type `MathOperation`:
   ```csharp
   public delegate double MathOperation(double x, double y);
   ```
2. Opret tre statiske metoder i en klasse, som matcher delegatens signatur (f.eks. `Add`, `Subtract`, `Multiply`).
3. Opret en metode `ExecuteAndPrint(double a, double b, MathOperation operation)`, som modtager to tal samt delegaten, udfører beregningen og udskriver resultatet i konsollen.
4. Afprøv din løsning i `Main`-metoden i `Lektion03Opgave01Main.cs` ved at kalde `ExecuteAndPrint` med:
   - Navngivne metoder (f.eks. `Add`).
   - Et lambda-udtryk for division (f.eks. `(x, y) => x / y`).
   - Et lambda-udtryk for potenser (`Math.Pow`).

> [!TIP]
> Læg mærke til, hvordan `ExecuteAndPrint` ikke behøver at vide, *hvilken* matematisk formel der udføres – den kalder blot delegaten!

---

### Opgave 2: Opret passende Delegate (`LogHandler`)

**Fokus:** Analysere eksisterende metodekald og lambda-udtryk for at udlede argument- og returtyper til en manglende delegate.

**Beskrivelse:**  
Nedenstående kode fejler ved kompilering, fordi delegaten `LogHandler` endnu ikke er defineret. Din opgave er at analysere koden, finde frem til den korrekte delegate-signatur og oprette den.

#### Udleveret kode:

```csharp
namespace Lektion03Opgave02;

public class LogProcessor
{
    // FEJL: 'LogHandler' findes ikke endnu!
    public void ProcessLogs(string[] logs, LogHandler handler)
    {
        foreach (var log in logs)
        {
            // Delegaten kaldes med en streng og et DateTime-objekt
            handler(log, DateTime.Now);
        }
    }
}

class Program
{
    static void Main()
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
```

**Opgaver:**
1. Undersøg i `ProcessLogs`-metoden, hvordan `handler` kaldes:
   - Hvor mange argumenter sendes der med i kaldet `handler(...)`?
   - Hvilke datatyper har disse argumenter?
   - Opfanges der en returværdi fra delegaten?
2. Opret den manglende `LogHandler` delegate med den korrekte signatur, så koden kompilerer og kører uden fejl.

---

### Opgave 3: Introduktion til `async`/`await` – Star Wars `UserRepository`

**Fokus:** Forståelse af `async`- og `await`-nøgleordene, returtyper (`Task` og `Task<T>`) samt asynkront metodekald uden blokering.

**Beskrivelse:**  
Klassen `UserRepository` er udleveret i projektet `Lektion03Opgave03`. Den indeholder en liste med 5 Star Wars karakterer og simulerer en langsom database- eller API-hentning med `Task.Delay(2000)`. Din opgave er at skrive asynkron kode i `Main`-metoden i `Lektion03Opgave03Main.cs`, der kalder `UserRepository` korrekt med `await`.

#### Udleveret kode (`UserRepository.cs`):

```csharp
namespace Lektion03Opgave03;

public class UserRepository
{
    private readonly List<string> _starWarsCharacters = new()
    {
        "Luke Skywalker",
        "Darth Vader",
        "Princess Leia",
        "Han Solo",
        "Yoda"
    };

    public async Task<string> GetUserByIdAsync(int id)
    {
        // Simulerer 2 sekunders database-/netværksventetid
        await Task.Delay(2000);

        int index = id - 1;
        if (index >= 0 && index < _starWarsCharacters.Count)
        {
            return _starWarsCharacters[index];
        }

        return "Ukendt karakter";
    }
}
```

**Opgaver:**
1. Gør `Main`-metoden asynkron ved at ændre signaturen til `static async Task Main(string[] args)`.
2. Instantiér `UserRepository`.
3. Udskriv en besked i konsollen før og efter kaldet til `GetUserByIdAsync` for at observere programmets afvikling.
4. Hent data for 3 Star Wars karakterer (f.eks. id 1, 2 og 5) og udskriv resultaterne med `await`. 
5. **Visualisering af ventetid (Loading-prikker):**  
   Ændr dit metodekald, så der udskrives et punktum (`.`) i konsollen hver 500 millisekunder, mens programmet venter på svar fra "databasen" (`UserRepository`).  
   *(Hint: `task.IsCompleted` er en boolean der fortæller om opgaven er fuldført).*

> [!TIP]
> Læg mærke til, at du skal bruge `await` foran `repository.GetUserByIdAsync(...)`. Prøv at undersøge, hvad der sker, hvis du udelader `await`!

---

### Opgave 4: Rigtige HTTP-kald med `HttpClient` og JSON

**Fokus:** Brug af virkelige asynkrone APIs i .NET (`HttpClient`), netværks-I/O samt asynkron deserialisering og fejlhåndtering.

**Beskrivelse:**  
I denne opgave skal du bygge et program, der henter tilfældige facts eller data fra et offentligt REST API over internettet ved hjælp af `HttpClient`. Du kan f.eks. bruge Cat Facts API (`https://catfact.ninja/fact`) eller et lignende åbent JSON API.

**Opgaver:**
1. Opret en instans af `HttpClient`.
2. Skriv en asynkron metode `FetchCatFactAsync()`, der har returtypen `Task<string>` (eller returnerer et DTO-objekt/record):
   - Brug `HttpClient.GetFromJsonAsync<CatFactDto>()` eller `HttpClient.GetStringAsync()` til asynkront at hente data fra `https://catfact.ninja/fact`.
   - Indpak HTTP-kaldet i en `try-catch` blok for at opfange og håndtere eventuelle netværksfejl (`HttpRequestException`).
3. Kald metoden fra din asynkrone `Main`-metode og udskriv det hentede resultat i konsollen.

> [!NOTE]
> `HttpClient` er designet til at blive genbrugt igennem applikationens levetid frem for at blive oprettet og lukket ved hvert enkelt kald.
