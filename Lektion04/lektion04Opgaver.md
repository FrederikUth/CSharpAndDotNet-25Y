# Opgaver: Lektion 04 – Controller-based Web API & Scalar

## Guide: Opsætning af Scalar API Reference via NuGet

For at få et interaktivt API-dokumentationsinterface, bruger vi **Scalar** (`Scalar.AspNetCore`) sammen med den indbyggede OpenAPI-støtte i .NET

### Trin 1: Tilføj NuGet-pakken `Scalar.AspNetCore`

Du kan installere pakken på to måder:

**Via Terminal / .NET CLI:**
Åbn din terminal i projektets mappe og kør:
```bash
dotnet add package Scalar.AspNetCore
```

**Via Visual Studio (NuGet Package Manager):**
1. Højreklik på dit projekt i *Solution Explorer* og vælg **Manage NuGet Packages...**
2. Søg efter `Scalar.AspNetCore`.
3. Vælg pakken og klik på **Install**.

---

### Trin 2: Konfigurer `Program.cs`

Åbn din `Program.cs` og sørg for følgende opsætning:

```csharp
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Tilføj Controller-tjenester til DI containeren
builder.Services.AddControllers();

// 2. Tilføj OpenAPI-støtte (krævet af Scalar)
builder.Services.AddOpenApi();

var app = builder.Build();

// 3. Konfigurer OpenAPI og Scalar UI i Development-miljøet
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // Viser interaktivt Scalar UI på /scalar/v1
}

app.UseHttpsRedirection();

// 4. Mappe controller-routes
app.MapControllers();

app.Run();
```

> [!TIP]
> Når du starter projektet i `Development`-miljøet, kan du tilgå din interaktive API-dokumentation ved at navigere til URL'en `https://localhost:<port>/scalar/v1` i din browser.

---

## Opgave 1: Task Tracker API (To-Do List)

**Fokus:** Controller-baseret Web API, `ControllerBase`, `[ApiController]`, Attribute Routing, HTTP Verbs (`GET`, `POST`, `PUT`, `DELETE`), DTOs og `ActionResult<T>`.

**Beskrivelse:**  
I denne opgave skal du bygge et RESTful Web API til at styre en To-Do liste. Opgaven skal implementeres i en ny controller kaldet `TasksController` (`TasksController.cs`).

### 1. Model & DTOs
Opret en model og de nødvendige DTOs for en opgave:
- `TaskItem`:
  - `int Id`
  - `string Title`
  - `string? Description`
  - `bool IsCompleted`
  - `DateTime CreatedAt`
- Opret relevante DTOs (f.eks. `CreateTaskDto` til oprettelse og `UpdateTaskDto` til opdatering).

### 2. TaskController Implementation
Opret en `TasksController`, som arver fra `ControllerBase` og er dekoreret med `[ApiController]` og `[Route("api/[controller]")]`.

Implementer følgende endepunkter:

1. **`GET /api/tasks`** – Hent alle opgaver.
   - Returnerer en liste af opgaver med HTTP status `200 OK`.
2. **`GET /api/tasks/{id}`** – Hent specifik opgave via ID.
   - Returnerer `200 OK` med opgaven, hvis den findes.
   - Returnerer `404 Not Found`, hvis opgaven ikke eksisterer.
3. **`POST /api/tasks`** – Opret en ny opgave.
   - Modtager `CreateTaskDto` fra request body.
   - Tildeler et unikt `Id` og sætter `CreatedAt = DateTime.UtcNow`.
   - Returnerer `201 Created` med `CreatedAtAction` og `Location` header til det nye opgave-endepunkt.
4. **`PUT /api/tasks/{id}`** – Opdater en eksisterende opgave.
   - Modtager opdaterede data (`UpdateTaskDto`) og et `id`.
   - Opdaterer opgavens egenskaber (f.eks. markerer som udført `IsCompleted = true`).
   - Returnerer `204 No Content` ved succes, eller `404 Not Found` hvis opgaven ikke findes.
5. **`DELETE /api/tasks/{id}`** – Slet en opgave.
   - Sletter opgaven med det angivne `id`.
   - Returnerer `204 No Content` ved succes, eller `404 Not Found` hvis opgaven ikke findes.

> [!NOTE]
> Til denne opgave kan du benytte en in-memory liste (`private static readonly List<TaskItem> _tasks = new();` eller en in-memory repository-klasse) til at opbevare data, mens applikationen kører.

---

### Bonusudfordringer

1. **Filtrering via Query Parameter:**  
   Udvid `GET /api/tasks` så du kan filtrere på status via query parameters, f.eks.:  
   `GET /api/tasks?isCompleted=true`
2. **Model Validering:**  
   Tilføj Data Annotations (f.eks. `[Required]` og `[StringLength]`) på din `CreateTaskDto` og test i Scalar, at API'et automatisk returnerer `400 Bad Request`, hvis `Title` udelades.
3. **Scalar Berigelse:**  
   Tilføj `[ProducesResponseType]` attributter til dine controller action metoder, så Scalar dokumentationen præcist viser de mulige retursvar (`200`, `201`, `404`, `400`).
