---
marp: true
theme: default
paginate: true
header: 'Asynkron Programmering i C#'
footer: 'C# & .NET'
---

# Asynkron Programmering i C#
### Non-blocking I/O, `async`/`await`, `Task<T>` og Concurrency

---

## Indholdsfortegnelse

1. **Hvorfor Asynkron Programmering?**
2. **Synkron vs. Asynkron (I/O-bound vs. CPU-bound)**
3. **Broen fra JavaScript til C# (`Promise` vs `Task`)**
4. **Grundlæggende Syntaks: `async` & `await`**
5. **Returtyper i Async Metoder (`Task`, `Task<T>`, `ValueTask<T>`, `void`)**
6. **Hvad sker der under motorhjelmen? (State Machine)**

---

7. **`Thread.Sleep` vs. `Task.Delay`**
8. **Kombinering af Tasks (`Task.WhenAll` & `Task.WhenAny`)**
9. **Fejlhåndtering i Async Kode (`try-catch` & exceptions)**
10. **Cancellation med `CancellationToken`**
11. **Asynkrone Strømme (`IAsyncEnumerable<T>`)**
12. **Faldgruber & Anti-Patterns (Do's & Don'ts)**
13. **Samlet Praktisk Eksempel**
14. **Opsummering**

---

## 1. Hvorfor Asynkron Programmering?

I moderne applikationer venter kode ofte på eksterne ressourcer:
- **Netværkskald** (HTTP REST APIs, gRPC, mikrotjenester)
- **Databaseforespørgsler** (EF Core, SQL Server, MongoDB)
- **Fil-I/O** (Læsning og skrivning på disk)

---

### Uden Asynkron programmering (Synkron / Blokering):
- En tråd i trådpuljen (ThreadPool) blokeres fuldstændigt, mens den venter på svar.
- **Skalerbarhedsproblem i Web APIs**: Serveren løber tør for tråde (*Thread Starvation*).
- **UI-problemer**: Brugerfladen fryser, mens en fil indlæses.

---

### Med Asynkron programmering (Non-blocking):
- Tråden frigives til at udføre andet arbejde, mens I/O-operationen afventes på operativsystemniveau (via *I/O Completion Ports* - IOCP).
- Når operationen er færdig, genoptages koden på en ledig tråd.

---

## 2. Synkron vs. Asynkron Execution

### Synkron (Blokerende)
```
Tråd 1: [--- Udfører kode ---][====== Venter på DB/Netværk ======][--- Fortsætter ---]
                                (Tråden er låst og spildt!)
```

---

### Asynkron (Non-blocking)
```
Tråd 1: [--- Start I/O ---] (Tråden frigives til trådpuljen)
                             ... OS venter på hardware/netværk ...
Tråd 2:                     [--- Genoptag & behandl resultat ---]
```

> **Vigtig pointe**: Asynkron programmering handler **ikke** om at køre kode hurtigere eller skabe flere tråde. Det handler om at **udnytte eksisterende tråde effektivt** ved ikke at lade dem sidde og vente uvirksomt.

---

## 3. Broen fra JavaScript til C#

Hvis I kommer fra **JavaScript / TypeScript**, vil asynkron programmering føles meget velkendt, da JS adopterede C#'s `async`/`await` syntaks!

| Koncept | JavaScript / TypeScript | C# / .NET |
| :--- | :--- | :--- |
| **Fremtidigt resultat (Promise)** | `Promise<T>` | `Task<T>` / `ValueTask<T>` |
| **Async operation uden resultat** | `Promise<void>` | `Task` |
| **Syntaks for ventetid** | `async` / `await` | `async` / `await` |

---

| Koncept | JavaScript / TypeScript | C# / .NET |
| :--- | :--- | :--- |
| **Kombiner alle promises/tasks** | `Promise.all([p1, p2])` | `Task.WhenAll(t1, t2)` |
| **Vent på første resultat** | `Promise.race([p1, p2])` | `Task.WhenAny(t1, t2)` |
| **Tråd-model** | Single-threaded Event Loop | Multi-threaded ThreadPool (trådpulje) |

> **Vigtig forskel**: JavaScript kører i ét single-threaded Event Loop. I C# afvikles asynkrone opgaver på en **multi-threaded ThreadPool**, hvilket betyder at koden før og efter et `await` kan blive udført på to forskellige tråde!

---

## 4. Grundlæggende Syntaks: `async` & `await`

For at gøre en metode asynkron benyttes to nøgleord:

1. **`async` modifier**: Markerer at metoden indeholder asynkron logik og tillader brug af `await`.
2. **`await` operator**: Pauser metodens udførelse (uden at blokere tråden), indtil den afventede `Task` er færdig.

---

```csharp
using System.Net.Http;

public class DataFetcher
{
    private readonly HttpClient _httpClient = new();

    // Metodesignatur: async + returnerer Task<string>
    public async Task<string> FetchDataAsync(string url)
    {
        // await frigiver tråden her indtil HTTP-responsen modtages
        string result = await _httpClient.GetStringAsync(url);
        
        return result.ToUpper(); // Returnerer string, som automatisk pakkes i Task<string>
    }
}
```

---

## 5. Returtyper i Async Metoder

C# tilbyder 4 primære returtyper for `async`-metoder:

### 1. `Task<T>`
Anvendes når metoden returnerer en værdi af typen `T`.
```csharp
public async Task<int> CalculateSumAsync(int a, int b) { ... }
```

### 2. `Task`
Anvendes når metoden **ikke** returnerer en værdi (svarer til `void` i synkron kode).
```csharp
public async Task SaveToDatabaseAsync(User user) { ... }
```

---

### 3. `ValueTask<T>` / `ValueTask`
En `struct`-baseret task til high-performance scenerier for at undgå garbage collection (GC) allokeringer, når resultatet ofte allerede er tilgængeligt synkront (f.eks. fra en cache).

---

## Returtyper: Advarsel om `async void`

### 4. `async void` ⚠️ (Brug kun til Event Handlers!)
```csharp
// KUN tilladt i UI / Event Handlers!
private async void OnButtonClicked(object sender, EventArgs e)
{
    await ProcessDataAsync();
}
```

---

> ❌ **Hvorfor `async void` er farligt i normal kode:**
> - Kalderen kan **ikke `await`** metoden.
> - Undtagelser (exceptions) i en `async void` metode kan **ikke gribes** af en try-catch i kalderen og vil ofte **crashe hele applikationen**!

---

## 6. Hvad sker der under motorhjelmen?

Hvad gør C#-kompilatoren, når den ser `async` og `await`?

1. **Tilstandsmaskine (State Machine)**: Kompilatoren omskriver din metode til en `struct`, der implementerer `IAsyncStateMachine`.
2. **Opsplitning ved `await`**: Hvert `await`-punkt opdeler metoden i bidder.
3. **Capture Context**: `SynchronizationContext` gemmes (vigtigt i UI-apps som WPF/WinForms for at returnere til UI-tråden).

---

```
[Metode start] ──> [Udfør indtil await] ──> [Start I/O Task]
                                                 │
[Fortsæt logik] <── [Genoptag på ThreadPool] <───┘ (I/O færdig)
```

Ingen tråd sidder fast i venteposition – alt håndteres via callbacks registreret af tilstandsmaskinen.

---

## 7. `Thread.Sleep` vs. `Task.Delay`

Det er afgørende at kende forskellen på synkron og asynkron ventetid:

### ❌ `Thread.Sleep(1000)` – Synkron blokering
Låser den nuværende tråd i 1 sekund. Tråden kan **intet** andet lave i mellemtiden.

### ✅ `Task.Delay(1000)` – Asynkron ventetid
Opretter en timer og frigiver tråden med det samme. Efter 1 sekund placeres fortsættelsen i trådpuljen.

---

```csharp
// DÅRLIGT: Blokerer tråden
public async Task BadDelayAsync()
{
    Thread.Sleep(2000); // ❌ Blokerer!
}

// GODT: Frigiver tråden
public async Task GoodDelayAsync()
{
    await Task.Delay(2000); // ✅ Non-blocking!
}
```

---

## 8. Kombinering af Tasks (Parallel Concurrency)

Hvis du skal udføre flere asynkrone operationer, kan du starte dem samtidigt i stedet for sekventielt.

### Sekventiel udførelse (Langsommere)
```csharp
// Venter 2 sekunder + 2 sekunder = ~4 sekunder totalt
string user = await FetchUserAsync();
string orders = await FetchOrdersAsync();
```

---

### Samtidig udførelse med `Task.WhenAll` (Hurtigere)
```csharp
// Starter begge tasks i baggrunden samtidigt
Task<string> userTask = FetchUserAsync();
Task<string> ordersTask = FetchOrdersAsync();

// Venter på at BÅDE userTask og ordersTask er færdige (~2 sekunder totalt)
await Task.WhenAll(userTask, ordersTask);

string user = await userTask;
string orders = await ordersTask;
```

---

## `Task.WhenAny` – Vent på den første

`Task.WhenAny` returnerer så snart **én** af de afventede tasks er fuldført:

```csharp
Task<string> server1 = FetchFromMirror1Async();
Task<string> server2 = FetchFromMirror2Async();

// Vent på den hurtigste server
Task<string> completedTask = await Task.WhenAny(server1, server2);

// Hent resultatet fra den færdige task
string fastResult = await completedTask;
Console.WriteLine($"Fastest response: {fastResult}");
```

---

### Typiske anvendelser af `Task.WhenAny`:
- **Redundante kald**: Hent fra det hurtigste API spejl.
- **Timeouts**: Vent på enten et opgave-kald eller en `Task.Delay(timeout)`.

---

## 9. Fejlhåndtering i Async Kode

Fejlhåndtering i `async`/`await` ligner almindelig `try-catch`, fordi kompilatoren udpakker undtagelser for dig ved `await`.

### Enkelt `await`
```csharp
try
{
    string data = await FetchDataFromApiAsync("https://invalid-url.com");
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Netværksfejl: {ex.Message}");
}
```

---

### Håndtering ved `Task.WhenAll`
Når du awaiter `Task.WhenAll`, vil `await` kun kaste den **første** exception, der opstod. Hvis du vil se alle exceptions, tilgås taskens `.Exception` ejendom:

```csharp
Task t1 = Task.FromException(new InvalidOperationException("Fejl 1"));
Task t2 = Task.FromException(new ArgumentException("Fejl 2"));
Task allTasks = Task.WhenAll(t1, t2);

try { await allTasks; }
catch {
    // allTasks.Exception indeholder en AggregateException med alle fejl
    foreach (var inner in allTasks.Exception!.InnerExceptions)
    {
        Console.WriteLine($"Grebet fejl: {inner.Message}");
    }
}
```

---

## 10. Cancellation med `CancellationToken`

Asynkrone operationer kan tage lang tid eller blive overflødige (f.eks. ved brugersøgninger eller HTTP request timeouts). I .NET håndteres dette med **`CancellationToken`**.

### Koncept:
- **`CancellationTokenSource` (CTS)**: Styringsenheden som udsteder afbrydelsessignalet (`cts.Cancel()`).
- **`CancellationToken` (CT)**: Token som sendes med ned i asynkrone metoder.

---

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3)); // Timeout efter 3 sek.

try
{
    await DownloadLargeFileAsync("https://example.com/file.zip", cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Download blev afbrudt pga. timeout eller brugerens ønske.");
}
```

---

## Implementering af Cancellation i egne metoder

Du bør altid videresende `CancellationToken` til underliggende async metoder eller tjekke token manuelt:

```csharp
public async Task ProcessItemsAsync(List<string> items, CancellationToken cancellationToken = default)
{
    foreach (var item in items)
    {
        // Tjek om afbrydelse er anmodet (kaster OperationCanceledException hvis sandt)
        cancellationToken.ThrowIfCancellationRequested();

        // Send token videre til andre async kald
        await Task.Delay(500, cancellationToken);
        Console.WriteLine($"Behandlet: {item}");
    }
}
```

---

## 11. Asynkrone Strømme (`IAsyncEnumerable<T>`)

Introduceret i C# 8.0. Tillader streaming af data asynkront (f.eks. linje-for-linje fra netværk eller database uden at indlæse alt i hukommelsen først).

### Producent: `IAsyncEnumerable<T>` med `yield return`
```csharp
public async IAsyncEnumerable<int> GenerateNumbersAsync()
{
    for (int i = 1; i <= 5; i++)
    {
        await Task.Delay(500); // Simulér asynkrona data-hentning
        yield return i;
    }
}
```

---

### Konsument: `await foreach`
```csharp
await foreach (int number in GenerateNumbersAsync())
{
    Console.WriteLine($"Modtog tal: {number}");
}
```

---

## 12. Faldgruber & Anti-Patterns (Do's & Don'ts)

### 1. Synchronous over Asynchronous (Sync-over-Async) ⛔
Brug **aldrig** `.Result` eller `.Wait()` på en Task!
```csharp
// ❌ DÅRLIGT: Risiko for DEADLOCKS og Thread Pool Starvation!
string data = FetchDataAsync().Result; 
FetchDataAsync().Wait();

// ✅ GODT: Async all the way down!
string data = await FetchDataAsync();
```

---

### 2. Async Void ⛔
```csharp
// ❌ DÅRLIGT: Ubehandlede exceptions vil crashe appen
public async void Process() { ... }

// ✅ GODT:
public async Task ProcessAsync() { ... }
```

---

## Faldgruber & Anti-Patterns (Fortsat)

### 3. Glemt `await` på fire-and-forget ⚠️
```csharp
// ❌ DÅRLIGT: Hvis SaveToDbAsync kaster en fejl, bliver den opfanget som ubehandlet!
SaveToDbAsync(data); 

// ✅ GODT: Await kaldet
await SaveToDbAsync(data);
```

---

### 4. Forveksling af CPU-bound og I/O-bound arbejde
- **I/O-bound** (Netværk, DB, Disk): Brug eksisterende async APIs (`await _http.GetAsync()`). Opret **ikke** `Task.Run()` inde i biblioteksmetoder!
- **CPU-bound** (Tung beregning, billedbehandling): Brug `Task.Run(() => HeavyCalculation())` for at skubbe arbejdet til en baggrundstråd.

---

## 13. Samlet Praktisk Eksempel

Her er et komplet, realistisk eksempel der viser brug af `HttpClient`, `CancellationToken`, og fejlhåndtering:

```csharp
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

```

---

```csharp
        try
        {
            Console.WriteLine($"[INFO] Henter vejrdata for {city}...");
            
            // Send request med cancellation support
            HttpResponseMessage response = await _client.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync(cancellationToken);
            Console.WriteLine($"[SUCCESS] Data modtaget: {json.Substring(0, Math.Min(30, json.Length))}...");
        }

```

---

```csharp
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
```

---

## Eksempel: Afvikling af WeatherService

```csharp
public class Program
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
```

---

## 14. Opsummering & Best Practices

1. **Async all the way down**: Kald async metoder fra async metoder hele vejen op til `Main` eller controller/action.
2. **Undgå `.Result` og `.Wait()`**: De blokerer tråden og kan forårsage deadlocks.
3. **Brug `Task` frem for `async void`**: Reserver `async void` udelukkende til event handlers.
4. **Parallellisér uafhængige opgaver**: Brug `Task.WhenAll` frem for sekventiel `await` når opgaver ikke afhænger af hinanden.
5. **Støt Cancellation**: Videresend altid `CancellationToken` i dine asynkrone APIs.
6. **Brug `Task.Delay` frem for `Thread.Sleep`**: Undgå at blokere trådpuljens tråde uhensigtsmæssigt.

---

# Spørgsmål & Diskussion 🚀