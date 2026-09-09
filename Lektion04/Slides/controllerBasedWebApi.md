---
marp: true
theme: default
paginate: true
header: 'Controller-based Web API i .NET'
footer: 'C# & .NET | Lektion 04'
---

# Controller-based Web API i .NET

## Byg skalerbare, strukturerede og typesikre RESTful Web APIs med ASP.NET Core Controllers

---

## Indholdsfortegnelse

1. **Hvad er et Web API i .NET?**
2. **Controllers vs. Minimal APIs**
3. **Konfiguration & Setup i `Program.cs`**
4. **Anatomi af en Controller (`[ApiController]` & `ControllerBase`)**
5. **Routing & HTTP Verbs**
6. **Model Binding & Binding Sources (`[FromBody]`, `[FromQuery]`, m.fl.)**
7. **Returtyper (`IActionResult` vs. `ActionResult<T>`)**

---

## Indholdsfortegnelse (fortsat)

8. **Model Validation & Data Annotations**
9. **Dependency Injection (DI) i Controllers**
10. **Data Transfer Objects (DTOs) & Separering af Lag**
11. **Samlet Praktisk Eksempel (Full CRUD Controller)**
12. **Best Practices & Opsummering**

---

## 1. Hvad er et Web API i .NET?

- **ASP.NET Core Web API** er .NETs framework til at bygge HTTP-baserede services (REST APIs).
- Leverer data (typisk som **JSON** eller XML) til klienter:
  - Frontend applikationer (React, Angular, Vue, Blazor)
  - Mobilapps (iOS, Android, MAUI)
  - Andre backend mikrotjenester
- **Controller-baseret arkitektur**:
  - Organiserer relaterede HTTP endpoints i strukturerede C# klasser (*Controllers*).
  - Baseret på det klassiske MVC-mønster (Model-View-Controller), hvor View'et blot er erstattet af JSON/data.

---

## 2. Controllers vs. Minimal APIs

I moderne .NET (fra .NET 6+) har vi to måder at bygge Web APIs på:

| Egenskab | Controller-based Web API | Minimal API |
| :--- | :--- | :--- |
| **Struktur** | Klasse-baseret, opdelt i filer | Lambdas / inline i `Program.cs` |
| **Egnet til** | Mellem/Store applikationer, enterprise | Små microservices, enkle APIs |
| **Konventioner** | Høje (Routing, Binding, Validation) | Eksplicitte / Minimalistiske |
| **Filters & Attributter** | Fuld support for Action Filters m.m. | Endpoint Filters |
| **Organisering** | Automatisk via DI & reflection | Kræver manuel opdeling ved vækst |

> **Konklusion**: Controller-baserede APIs giver en velkendt, konsistent og stærkt struktureret ramme til større projekter.

---

## 3. Konfiguration & Setup i `Program.cs`

For at aktivere controllers i et ASP.NET Core Web API skal to ting konfigureres i `Program.cs`:

```csharp
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Tilføj Controller-tjenester til Dependency Injection containeren
builder.Services.AddControllers();

// (Valgfrit) Tilføj OpenAPI & Scalar API Reference support
builder.Services.AddOpenApi();

var app = builder.Build();
```

---

```csharp
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // Viser interaktivt Scalar UI (standard på /scalar/v1)
}

app.UseHttpsRedirection();

// 2. Mappe controller-routes i HTTP pipeline
app.MapControllers();

app.Run();
```

---

## 4. Anatomi af en Controller

En typisk controller i ASP.NET Core arver fra `ControllerBase` og dekoreres med attributter:

```csharp
using Microsoft.AspNetCore.Mvc;

namespace MyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(new[] { "Product 1", "Product 2" });
    }
}
```

---

## `[ApiController]` Attributten – Hvad gør den?

Dekorering med `[ApiController]` aktiverer en række magiske konventioner:

1. **Automatisk Model Validation**: Hvis indkommende data validering mislykkes, returneres automatisk `400 Bad Request` før metoden overhovedet rammes.
2. **Automatisk Binding Source Inferencing**:
   - `[FromBody]` antages for komplekse typer.
   - `[FromRoute]` antages for parametre der matcher route-skabelonen.
   - `[FromQuery]` antages for primitive typer.
3. **ProblemDetails (RFC 7807)**: Fejlsvar returneres automatisk i et standardiseret JSON-format.
4. **Kræver Attribute Routing**: Tvinger brug af `[Route]` attributter på klassen eller metoderne.

---

## `ControllerBase` vs. `Controller`

Når du bygger **Web APIs**, arver du altid fra `ControllerBase`.

- **`ControllerBase`**:
  - Indeholder alt nødvendigt for Web APIs (kendskab til `HttpContext`, `Request`, `Response`, samt helper-metoder som `Ok()`, `NotFound()`, `BadRequest()`, `CreatedAtAction()`).
- **`Controller`**:
  - Arver fra `ControllerBase`, men tilføjer support for **Razor Views** (`View()`, `ViewData`, `ViewBag`).
  - **Undgå `Controller` i Web APIs**, da det bringer ubrugt MVC-view-overhead med sig.

---

## 5. Routing i Controllers

Routing definerer, hvilken HTTP-sti (URL) der rammer hvilken action-metode.

### Attribute Routing (`[Route]`)
Stien angives direkte over controlleren eller metoden:

```csharp
[ApiController]
[Route("api/[controller]")] // Sti: api/products (hvis klassen hedder ProductsController)
public class ProductsController : ControllerBase
{
    // GET api/products
    [HttpGet]
    public IActionResult GetAll() => Ok();

    // GET api/products/5
    [HttpGet("{id:int}")]
    public IActionResult GetById(int id) => Ok();
}
```

> **Token Replacement**: `[controller]` erstattes automatisk med klassens navn minus "Controller" (f.eks. `Products`).

---

## HTTP Verbs & Routing Attributter

ASP.NET Core leverer specifikke attributter til hver HTTP-metode:

| HTTP Verb | Attribut | Anvendelse / Betydning |
| :--- | :--- | :--- |
| **GET** | `[HttpGet]` | Hente ressource(r) (Idempotent & Safe) |
| **POST** | `[HttpPost]` | Oprette en ny ressource |
| **PUT** | `[HttpPut]` | Erstatte en eksisterende ressource (Idempotent) |
| **PATCH** | `[HttpPatch]` | Delvis opdatering af en ressource |
| **DELETE** | `[HttpDelete]` | Slette en ressource (Idempotent) |

---

## Route Constraints (Parameter Validering i Sti)

Du kan tilføje datatypesikring direkte i route-skabelonen:

```csharp
// Kun tal accepteres som id
[HttpGet("{id:int}")]
public IActionResult GetById(int id) { ... }

// GUID validering
[HttpGet("by-guid/{id:guid}")]
public IActionResult GetByGuid(Guid id) { ... }

// Interval & Længde
[HttpGet("range/{id:int:range(1, 100)}")]
public IActionResult GetInRange(int id) { ... }
```

Hvis kravene i constrainten ikke opfyldes (f.eks. `/api/products/abc`), returnerer ASP.NET Core automatisk `404 Not Found` (da routen ikke matcher).

---

## 6. Model Binding & Binding Sources

**Model Binding** er processen hvor ASP.NET Core automatisk tager data fra en HTTP-forespørgsel og konverterer det til C# parametre.

### Hvor kan data komme fra?
1. **`[FromBody]`**: Læses fra HTTP Request Body (typisk JSON).
2. **`[FromRoute]`**: Læses fra URL-stien (f.eks. `/api/products/{id}`).
3. **`[FromQuery]`**: Læses fra Query String (f.eks. `/api/products?category=tech&page=2`).
4. **`[FromHeader]`**: Læses fra HTTP request headers (f.eks. `Authorization`, `X-Api-Key`).
5. **`[FromForm]`**: Læses fra Form Data (f.eks. fil-uploads med `IFormFile`).

---

## Eksempel: Model Binding i Praksis

```csharp
[HttpPost("{categoryId:int}")]
public IActionResult CreateProduct(
    [FromRoute] int categoryId,                      // Fra URL stien
    [FromQuery] bool publishImmediately,              // Fra query string (?publishImmediately=true)
    [FromBody] CreateProductDto dto,                 // Fra JSON body
    [FromHeader(Name = "X-User-Id")] string userId)  // Fra HTTP header
{
    // Alle værdier er nu automatisk parsede og typesikre!
    return Ok();
}
```

> Takket være `[ApiController]` behøver du ofte ikke skrive `[FromBody]` eller `[FromRoute]` eksplicit, da frameworket udleder det automatisk!

---

## 7. Returtyper i Controller Action Metoder

Der er tre måder at returnere svar fra en Controller action i ASP.NET Core:

1. **Konkret Type (`Product` / `IEnumerable<Product>`)**
   - Returnerer direkte et objekt. Kan kun returnere `200 OK` eller `204 No Content`. Ingen fleksibilitet for HTTP statuskoder.
2. **`IActionResult`**
   - Returnerer et HTTP svar-objekt (f.eks. `Ok()`, `NotFound()`). Fleksibelt, men taber typesikkerhed i Swagger/OpenAPI uden ekstra attributter.
3. **`ActionResult<T>` (Anbefalet Best Practice)**
   - Kombinerer det bedste fra begge verdener! Tillader enten at returnere en instans af `T` eller et `IActionResult`.

---

## `ActionResult<T>` i Praksis

```csharp
[HttpGet("{id:int}")]
public async Task<ActionResult<ProductDto>> GetById(int id)
{
    var product = await _productService.GetByIdAsync(id);

    if (product == null)
    {
        // Returnerer 404 Not Found (IActionResult)
        return NotFound(new { Message = $"Produkt med id {id} blev ikke fundet." });
    }

    // Returnerer 200 OK med ProductDto (Implicit konvertering til ActionResult<ProductDto>)
    return Ok(product);
}
```

### Hvorfor `ActionResult<T>`?
- Fuld typesikkerhed for OpenApi documentation.
- Mulighed for at returnere forskellige HTTP statuskoder (`200`, `400`, `404`, `500`).

---

## Indbyggede Helper Metoder i `ControllerBase`

`ControllerBase` indeholder et væld af hjælpemetoder til at bygge HTTP-svar:

| Statuskode | Helper Metode | Beskrivelse |
| :--- | :--- | :--- |
| **200 OK** | `Ok(data)` | Succesfuld hentning eller udførelse med data |
| **201 Created** | `CreatedAtAction(...)` | Ressource oprettet. Inkluderer `Location` header |
| **204 No Content**| `NoContent()` | Operation lykkedes, intet data at returnere |
| **400 Bad Request**| `BadRequest(errors)` | Klientfejl / ugyldige input-data |
| **401 Unauthorized**| `Unauthorized()` | Manglende autentificering |
| **403 Forbidden** | `Forbid()` | Autentificeret, men mangler rettigheder |
| **404 Not Found** | `NotFound()` | Ressource ikke fundet |

---

## 8. Model Validation & Data Annotations

Før en ressource gemmes, skal indkommende data valideres. .NET understøtter **Data Annotations** direkte på DTOs/modeller.

```csharp
public record CreateProductDto(
    [Required(ErrorMessage = "Navn er påkrævet")]
    [StringLength(100, MinimumLength = 3)]
    string Name,

    [Range(0.01, 100000.0, ErrorMessage = "Pris skal være større end 0")]
    decimal Price,

    [EmailAddress]
    string ContactEmail
);
```

---

## Automatisk Validering med `[ApiController]`

Når en controller er markeret med `[ApiController]`:

- ASP.NET Core validerer automatisk modtagede objekter mod deres Data Annotations **før** action-metoden kaldes.
- Hvis valideringen fejler, afbrydes forespørgslen med det samme og returnerer **`400 Bad Request`**:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": [ "Navn er påkrævet" ],
    "Price": [ "Pris skal være større end 0" ]
  }
}
```

 Du slipper for manuelt at skrive `if (!ModelState.IsValid) return BadRequest();`!

---

## 9. Dependency Injection (DI) i Controllers

ASP.NET Core Controllers benytter **Constructor Injection** til at modtage afhængigheder.

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    // Afhængigheder indsprøjtes automatisk via DI containeren
    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        _logger.LogInformation("Henter alle produkter");
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }
}
```

---

## Registrering af Services i `Program.cs`

For at DI containeren kan indsprøjte `IProductService`, skal den registreres med en levetid (*Lifetime*):

```csharp
// 1. Transient: Ny instans HVER gang den efterspørges
builder.Services.AddTransient<ITokenService, TokenService>();

// 2. Scoped (Anbefalet til Repositories/Services i Web APIs):
// Ny instans pr. HTTP Request
builder.Services.AddScoped<IProductService, ProductService>();

// 3. Singleton: Én enkelt instans i hele applikationens levetid
builder.Services.AddSingleton<ICacheService, CacheService>();
```

> **Husk**: Controllers skabes som **Scoped** pr. HTTP request i ASP.NET Core.

---

## 10. Data Transfer Objects (DTOs)

**Gylne Regel i Web APIs**: Eksponer aldrig dine Database Entities (f.eks. EF Core klasser) direkte i din Controller!

### Hvorfor bruge DTOs?
1. **Sikkerhed (Over-Posting)**: Undgå at klienter kan opdatere følsomme felter (f.eks. `IsAdmin` eller `PasswordHash`).
2. **Koblede afhængigheder**: Ændringer i databasestrukturen ødelægger ikke API-kontrakten for klienterne.
3. **Ydeevne**: Undgå cirkulære JSON-referencer og overfør kun nødvendige data over netværket.

---

## DTO Mønster med C# `record`

C# `record` er ideel til DTOs da de er uforanderlige (*immutable*) og har indbygget værdibaseret sammenlignment:

```csharp
// Input DTO til oprettelse
public record CreateProductDto(string Name, decimal Price, string Description);

// Input DTO til opdatering
public record UpdateProductDto(string Name, decimal Price, string Description);

// Output Response DTO
public record ProductDto(int Id, string Name, decimal Price, string Description, DateTime CreatedAt);
```

---

## 12. Dokumentation med OpenAPI / Scalar

OpenAPI genererer interaktiv API-dokumentation og test-UI (som **Scalar**) automatisk.

### Berig din Controller med Attributter
Brug `[ProducesResponseType]` til at beskrive mulige HTTP statuskoder for OpenAPI og Scalar:

```csharp
[HttpGet("{id:int}")]
[ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public async Task<ActionResult<ProductDto>> GetById(int id)
{
    var product = await _productService.GetByIdAsync(id);
    if (product == null) return NotFound();
    return Ok(product);
}
```

---

## 11. Samlet Praktisk Eksempel: Full CRUD Controller

Lad os samle alt i en komplet, ren og velstruktureret `ProductsController`:

```csharp
using Microsoft.AspNetCore.Mvc;

namespace MyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // GET: api/products
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }
```

---

## Samlet Praktisk Eksempel (Fortsat: GET & POST)

```csharp
    // GET: api/products/5
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    // POST: api/products
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto)
    {
        var createdProduct = await _productService.CreateAsync(dto);
        
        // Returnerer 201 Created samt Location-header til api/products/{id}
        return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
    }
```

---

## Samlet Praktisk Eksempel (Fortsat: PUT & DELETE)

```csharp
    // PUT: api/products/5
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
    {
        var updated = await _productService.UpdateAsync(id, dto);
        if (!updated) return NotFound();
        return NoContent(); // 204 No Content ved succesfuld opdatering uden returdata
    }

    // DELETE: api/products/5
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _productService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
```

---

## 12. Best Practices for Controller Web APIs

1. **Keep Controllers Thin**:
   - Controllerens eneste ansvar er HTTP (routing, request parsing, statuskoder).
   - Forretningslogik hører til i Services/MediatR/Domain lag.
2. **Brug `ActionResult<T>`**:
   - Giver både typesikkerhed for OpenAPI/Scalar og fleksibilitet til statuskoder.
3. **Brug Async/Await i henhold til I/O**:
   - Alle database- og I/O-kald skal være `async Task<...>`.
4. **Brug DTOs**:
   - Læk aldrig EF Core Entities direkte til klienten.
5. **Standardiser HTTP Statuskoder**:
   - `200` (OK), `201` (Created), `204` (No Content), `400` (Bad Request), `404` (Not Found).

---

## Opsummering

- **Controllers** giver en veldefineret, skalerbar struktur til enterprise Web APIs.
- **`[ApiController]`** giver gratis automatisk validering, binding source inferencing og `ProblemDetails`.
- **Routing & Model Binding** gør det nemt og typesikkert at modtage argumenter fra stier, query strings og JSON bodies.
- **Dependency Injection** sikrer løs kobling og testbarhed.
- **OpenAPI & Scalar** beriges enkelt via `[ProducesResponseType]` og returtypen `ActionResult<T>`.

---

# Spørgsmål & Diskussion?
