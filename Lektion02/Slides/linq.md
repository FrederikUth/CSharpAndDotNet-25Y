---
marp: true
theme: default
paginate: true
header: 'LINQ i .NET'
footer: 'C# & .NET'
---

# LINQ i .NET
### Language Integrated Query – Deklarativ databehandling i C#

---

## Indholdsfortegnelse

1. **Hvad er LINQ?**
2. **Hvorfor bruge LINQ? (Imperativ vs. Deklarativ)**
3. **LINQ Syntakser (Method Syntax vs. Query Syntax)**
4. **Filtrering & Projektion (`Where`, `Select`)**
5. **Sortering (`OrderBy`, `ThenBy`)**
6. **Aggregering & Betingelser (`Count`, `Sum`, `Any`, `All`)**
7. **Element-operatorer (`First`, `FirstOrDefault`, `SingleOrDefault`)**
8. **Gruppering & Paging (`GroupBy`, `Skip`, `Take`)**
9. **Deferred Execution vs. Immediate Execution**
10. **Anonyme Typer & Projektion**
11. **Set-operatorer & Joins (`Distinct`, `Except`, `Join`)**
12. **Samlet Praktisk Eksempel**
13. **Opsummering & Best Practices**

---

## 1. Hvad er LINQ?

- **LINQ** står for **Language Integrated Query**.
- Integrerer forespørgsler direkte i C#-sproget som førsteklasses elementer.
- Giver én ensartet syntaks til at forespørge på mange typer datakilder:
  - **LINQ to Objects**: `List<T>`, arrays, in-memory kollektioner
  - **LINQ to Entities (EF Core)**: Relationsdatabaser (SQL Server, PostgreSQL)
  - **LINQ to XML / JSON**
- Giver fuld **typesikkerhed** i kompileringstiden og **IntelliSense** i IDE'en.

---

## 2. Hvorfor bruge LINQ?

### Imperativ tilgang (Traditionel `foreach`-løkke):
```csharp
List<int> numbers = new() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
List<int> evenNumbers = new();

foreach (var n in numbers)
{
    if (n % 2 == 0)
    {
        evenNumbers.Add(n);
    }
}
```

### Deklarativ tilgang (LINQ):
```csharp
// "Beskriv HVAD du ønsker, i stedet for HVORDAN løkken skal køres"
var evenNumbers = numbers.Where(n => n % 2 == 0).ToList();
```

- **Højere læsbarhed**: Kortere og mere hensigtsorienteret kode.
- **Færre fejl**: Undgår manuelle tilstandsvariabler og indekseringsfejl.

---

## 3. LINQ Syntakser

LINQ kan skrives på to forskellige måder i C#:

### 1. Method Syntax (Fluent API / Lambda-udtryk) – *Standard i praksis*
```csharp
var result = people
    .Where(p => p.Age >= 18)
    .OrderBy(p => p.Name);
```

---

### 2. Query Syntax (SQL-lignende syntaks)
```csharp
var result = from p in people
             where p.Age >= 18
             orderby p.Name
             select p;
```

> **Bemærk:** C#-compileren oversætter Query Syntax til Method Syntax bag kulisserne. Begge tilgange yder nøjagtig det samme.

---

## 4. Filtrering & Projektion

### `Where` – Filtrering
Udvælger elementer, der opfylder en betingelse (et predikat).

```csharp
List<int> scores = new() { 45, 82, 93, 67, 88 };

var highScores = scores.Where(s => s >= 80);
// Resultat: 82, 93, 88
```

---

### `Select` – Projektion / Transformation
Transformerer hvert element fra én type/struktur til en anden.

```csharp
List<Person> people = GetPeople();

// Udtræk kun e-mailadresserne som en liste af strenge
IEnumerable<string> emails = people.Select(p => p.Email);

// Omdan tal til deres kvadratværdi
var squares = numbers.Select(x => x * x);
```

---

## 5. Sortering

### `OrderBy` / `OrderByDescending`
Sorterer samlingen efter en nøgle.

```csharp
var sortedByName = people.OrderBy(p => p.LastName);

var sortedByAgeDesc = people.OrderByDescending(p => p.Age);
```

---

### Secondary Sorting med `ThenBy`
Brug `ThenBy` / `ThenByDescending` til at sortere på ekstra felter.

```csharp
var sortedPeople = people
    .OrderBy(p => p.LastName)
    .ThenBy(p => p.FirstName)
    .ThenByDescending(p => p.Age);
```

---

## 6. Aggregering & Betingelser

### Aggregeringsmetoder:
```csharp
int count = numbers.Count();             // Antal elementer
double avg = numbers.Average();          // Gennemsnit
int sum = numbers.Sum();                 // Sum af alle tal
int max = numbers.Max();                 // Højeste værdi
int min = numbers.Min();                 // Laveste værdi
```

---

### Betingelsesmetoder (Returnerer `bool`):
```csharp
// Er der MINDST ÉT tal større end 50?
bool hasLarge = numbers.Any(n => n > 50);

// Er ALLE tal positive?
bool allPositive = numbers.All(n => n > 0);

// Indeholder listen tallet 7?
bool hasSeven = numbers.Contains(7);
```

---

## 7. Element-operatorer

Bruges til at udtrække et enkelt specifikt element fra samlingen.

| Metode | Ved intet match | Ved >1 match |
| :--- | :--- | :--- |
| `First()` | Kaster `InvalidOperationException` | Returnerer første match |
| `FirstOrDefault()` | Returnerer `default` (`null`/`0`) | Returnerer første match |
| `Single()` | Kaster `InvalidOperationException` | Kaster `InvalidOperationException` |
| `SingleOrDefault()`| Returnerer `default` | Kaster `InvalidOperationException` |

---

### Eksempel på Element-operatorer:

```csharp
// Find første bruger med det specifikke ID, eller null hvis ikke fundet
User? user = users.FirstOrDefault(u => u.Id == targetId);

if (user != null)
{
    Console.WriteLine($"Fundet: {user.Name}");
}

// Forventer præcis én primær nøgle match (kaster fejl ved dubletter)
User uniqueUser = users.Single(u => u.SocialSecurityNumber == ssn);
```

> **Regel:** Brug `FirstOrDefault()` når elementet kan mangle. Brug `SingleOrDefault()` når du forventer unikt match i databasen/listen.

---

## 8. Gruppering & Paging

### `GroupBy` – Gruppering
Opdeler data i grupper baseret på en nøgleværdi.

```csharp
var groupedByRole = employees.GroupBy(e => e.Department);

foreach (var group in groupedByRole)
{
    Console.WriteLine($"Afdeling: {group.Key} (Antal: {group.Count()})");
    foreach (var emp in group)
    {
        Console.WriteLine($"  - {emp.Name}");
    }
}
```

---

### Paging med `Skip` & `Take`
Ideel til pagination i API'er og brugerflader.

```csharp
int pageSize = 10;
int pageNumber = 3; // Tredje side

var pagedProducts = products
    .OrderBy(p => p.Name)
    .Skip((pageNumber - 1) * pageSize) // Springer de første 20 over
    .Take(pageSize);                  // Tager de næste 10
```

---

## 9. Deferred Execution (Forsinket Udførelse)

- De fleste LINQ-metoder udfører **IKKE** forespørgslen med det samme.
- Forespørgslen gemmes som en instruktion (`IEnumerable<T>`) og afvikles først, når samlingen **itereres** (`foreach`, `.ToList()`, `.ToArray()`).

```csharp
List<int> numbers = new() { 1, 2, 3 };

// Forespørgslen opbygges her – intet er beregnet endnu!
var query = numbers.Where(n => n > 1);

numbers.Add(4); // Tilføjes EFTER forespørgslen er defineret

// Forespørgslen afvikles først HER i løkken:
foreach (var n in query)
{
    Console.WriteLine(n); // Udskriver: 2, 3, 4
}
```

---

### Immediate Execution (Øjeblikkelig Udførelse)

Eksekver forespørgslen med det samme og gem resultatet i hukommelsen ved at kalde `.ToList()`, `.ToArray()` eller en aggregering (`.Count()`).

```csharp
// Forespørgslen eksekveres med det samme!
List<int> staticList = numbers.Where(n => n > 1).ToList();

numbers.Add(100); // Påvirker IKKE staticList!
```

> **Pas på Multiple Enumeration:** Genbruges en `IEnumerable` i flere `foreach`-løkker uden `.ToList()`, genafvikles databaserapporten/beregningen hver gang.

---

## 10. Anonyme Typer & Projektion

Kombinér `Select` med **anonyme typer** (`new { ... }`) for at skabe skræddersyede projektioner on-the-fly.

```csharp
var studentSummaries = students
    .Where(s => s.IsActive)
    .Select(s => new
    {
        StudentId = s.Id,
        FullName = $"{s.FirstName} {s.LastName}",
        IsPassed = s.Grade >= 02
    });

foreach (var s in studentSummaries)
{
    Console.WriteLine($"{s.StudentId}: {s.FullName} (Bestået: {s.IsPassed})");
}
```

---

## 11. Set-operatorer & Joins

### Set-operatorer:
```csharp
var uniqueNumbers = numbers.Distinct();               // Fjerner dubletter
var setAExceptB  = setA.Except(setB);                 // Elementer kun i A
var commonItems  = setA.Intersect(setB);              // Fælles elementer
var combined     = setA.Union(setB);                  // Unikt samlet sæt
```

---

### `Join` – Sammenfletning af datakilder:
```csharp
var customerOrders = orders.Join(
    customers,
    order => order.CustomerId,    // Outer key
    customer => customer.Id,       // Inner key
    (order, customer) => new       // Resultat-projektion
    {
        OrderId = order.Id,
        CustomerName = customer.Name,
        Amount = order.Amount
    }
);
```

---

## 12. Samlet Praktisk Eksempel

```csharp
public record Student(int Id, string Name, string Major, double GPA);

List<Student> students = new()
{
    new(1, "Anna", "Computer Science", 3.9),
    new(2, "Bjørn", "Mathematics", 2.8),
    new(3, "Cecilie", "Computer Science", 3.7),
    new(4, "David", "Computer Science", 3.2),
    new(5, "Elena", "Physics", 3.9)
};

// Find top CS-studerende med GPA >= 3.5, sorteret efter GPA og Navn
var topCSStudents = students
    .Where(s => s.Major == "Computer Science" && s.GPA >= 3.5)
    .OrderByDescending(s => s.GPA)
    .ThenBy(s => s.Name)
    .Select(s => new { s.Name, s.GPA });
```

---

## 13. Opsummeringsoversigt

| Metode | Formål | Returtype | Eksekvering |
| :--- | :--- | :--- | :--- |
| `Where` | Filtrering | `IEnumerable<T>` | Deferred |
| `Select` | Transformation / DTO mapping | `IEnumerable<TResult>` | Deferred |
| `OrderBy` / `ThenBy` | Sortering | `IOrderedEnumerable<T>` | Deferred |
| `GroupBy` | Gruppering efter nøgle | `IEnumerable<IGrouping>` | Deferred |
| `FirstOrDefault` | Hent første match eller null | `T?` | Immediate |
| `Any` / `All` | Tjek betingelser | `bool` | Immediate |
| `ToList` / `ToArray` | Materialisér samling i RAM | `List<T>` / `T[]` | Immediate |

---

## Best Practices & Opsummering

- **Brug Method Syntax** (`.Where().Select()`) som standard i moderne C#.
- **Vær bevidst om Deferred Execution**: Kald `.ToList()` når du har brug for et statisk øjebliksbillede af data.
- **Brug `FirstOrDefault()` frem for `First()`** når et søgeresultat må være tomt for at undgå uventede krak.
- **Brug projektion (`Select`)** til kun at hente de felter, du faktisk skal bruge (især vigtigt i Entity Framework Core).

### Spørgsmål? 🚀
