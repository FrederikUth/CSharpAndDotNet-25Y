---
marp: true
theme: default
paginate: true
header: 'Extension Members i C#'
footer: 'C# & .NET'
---

# Extension Members i C#
### Udvidelse af eksisterende typer uden modifikation eller nedarvning

---

## Indholdsfortegnelse

1. **Hvad er Extension Members / Methods?**
2. **Hvorfor bruge dem? (Motivation & Use Cases)**
3. **Grundlæggende syntaks (`static class` & `this`)**
4. **Hvordan virker det bag kulisserne?**
5. **Sammenhængen med LINQ**
6. **Extension Methods på Interfaces & Generics**
7. **Null-håndtering & Sikkerhed**
8. **Regler, Begrænsninger & Prioritering**
9. **Method Chaining & Fluent APIs**
10. **Praktiske Eksempler (String, DateTime, Collections)**

---

11. **Fremtidens C#: Nye Extension Members (Properties, Static m.fl.)**
12. **Best Practices (Do's & Don'ts)**
13. **Opsummering**

---

## 1. Hvad er Extension Members?

- Giver mulighed for at **"tilføje" metoder og medlemmer til eksisterende typer** uden:
  - At ændre i den oprindelige kildekode.
  - At oprette en nedarvet undertype (`subclass`).
  - At rekompilere det oprindelige bibliotek.
- Kan anvendes på:
  - **.NET indbyggede typer** (`string`, `int`, `DateTime`, osv.)
  - **Klasser fra 3.-parts biblioteker** (f.eks. NuGet-pakker)
  - **Interfaces** (`IEnumerable<T>`, `IComparable`, osv.)
  - **Forseglede klasser (`sealed`)**, der ellers ikke kan nedarves fra.

---

- Kaldes med **instans-syntaks**:
  ```csharp
  string email = "test@example.com";
  bool valid = email.IsValidEmail(); // Ligner en indbygget metode på string!
  ```

---

## 2. Hvorfor bruge dem?

### Uden Extension Methods:
Traditionelt samlede man hjælpefunktioner i statiske "Utility"- eller "Helper"-klasser:
```csharp
// Klodset læseretning og bryder "flow" i koden:
bool valid = StringUtils.IsValidEmail(email);
string shortText = TextHelper.Truncate(message, 50);
```

---

### Med Extension Methods:
```csharp
// Naturlig læseretning fra venstre mod højre:
bool valid = email.IsValidEmail();
string shortText = message.Truncate(50);
```

**Fordele:**
- **IntelliSense**: Metoderne dukker automatisk op i autocomplete i din IDE.
- **Højere læsbarhed**: Koden læses som naturlige handlinger på objektet.
- **Fluent API**: Let at kæde flere operationer sammen.

---

## 3. Grundlæggende Syntaks

Der er 3 faste regler for traditionelle Extension Methods:

1. Skal placeres i en **`static class`**.
2. Selve metoden skal være en **`static` metode**.
3. Den **første parameter** skal have nøgleordet **`this`** foran typen, der udvides.

---

```csharp
public static class StringExtensions
{
    // 'this string text' angiver, at metoden udvider string-typen
    public static int WordCount(this string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        return text.Split(new[] { ' ', '\t', '\n' }, 
            StringSplitOptions.RemoveEmptyEntries).Length;
    }
}
```

```csharp
// Anvendelse:
string quote = "C# er et fantastisk programmeringssprog";
int words = quote.WordCount(); // 5
```

---

## 4. Hvordan virker det bag kulisserne?

### Syntaktisk Sukker (Syntactic Sugar)

Extension methods er en compiler-finesse. C#-compileren omskriver dit instanskald til et helt almindeligt statisk metodekald:

```csharp
// Hvad du skriver:
int count = quote.WordCount();

// Hvad C#-compileren genererer (IL-kode):
int count = StringExtensions.WordCount(quote);
```

> **Vigtigt:**
> For at bruge en extension method skal det **namespace**, hvori din extension-klasse ligger, være importeret med `using MyNamespace;`.

---

## 5. Sammenhængen med LINQ

**LINQ to Objects er 100% bygget på Extension Methods!**

- Alle velkendte LINQ-metoder (`Where`, `Select`, `OrderBy`, `Any`, `GroupBy`, etc.) er defineret som extension methods i klassen `System.Linq.Enumerable`.

```csharp
namespace System.Linq
{
    public static class Enumerable
    {
        // Extension method på interfacet IEnumerable<TSource>
        public static IEnumerable<TSource> Where<TSource>(
            this IEnumerable<TSource> source, 
            Func<TSource, bool> predicate)
        {
            // Filtreringslogik...
        }
    }
}
```

---

Fordi metoden udvider `IEnumerable<T>`, bliver LINQ automatisk tilgængeligt på `List<T>`, `T[]`, `HashSet<T>`, `Dictionary<TKey, TValue>` osv.

---

## 6. Extension Methods på Interfaces & Generics

Ved at lave extension methods på et **interface** eller med **generics** kan funktionaliteten genbruges på tværs af mange datatyper:

```csharp
public static class CollectionExtensions
{
    // Udvider ethvert IEnumerable<T> med en tjek-metode
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
    {
        return source == null || !source.Any();
    }

    // Generic constraint: Kun for typer der implementerer IComparable<T>
    public static bool IsBetween<T>(this T value, T min, T max) where T : IComparable<T>
    {
        return value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;
    }
}
```

---

```csharp
List<int>? numbers = null;
bool empty = numbers.IsNullOrEmpty(); // true

int age = 25;
bool inRange = age.IsBetween(18, 65); // true
```

---

## 7. Null-håndtering & Sikkerhed

En bemærkelsesværdig egenskab: **Extension methods kan kaldes på en `null`-reference!**

```csharp
string? name = null;

// Traditionel instansmetode -> Kaster NullReferenceException!
// name.ToUpper();

// Extension method -> Kaster IKKE fejl ved kaldet:
bool result = name.IsNullOrEmpty(); 
```

---

### Hvorfor crasher det ikke?
Fordi kaldet omskrives til `CollectionExtensions.IsNullOrEmpty(name)`. Værdien `null` gives blot videre som argument til `source`.

> **Best Practice:**
> Tjek altid dine parametre for `null` inde i metoden (`ArgumentNullException.ThrowIfNull(...)` eller `if (source is null)`).

---

## 8. Regler, Begrænsninger & Prioritering

### 1. Instansmetoder vinder altid
Hvis en klasse har en instansmetode med **nøjagtig samme navn og signatur**, kaldes instansmetoden altid frem for din extension method:
```csharp
public class Person {
    public void SayHello() => Console.WriteLine("Fra klassen");
}
// Extension med samme signatur bliver ALDRIG kaldt for Person!
```

---

### 2. Ingen adgang til private felter
- Extension methods har **ikke** adgang til `private` eller `protected` felter/metoder i typen.
- De kan udelukkende benytte typens offentlige (`public`) API.

### 3. Namespace prioritet
- Extensions i det lokale namespace vælges før extensions fra importerede `using` namespaces.

---

## 9. Method Chaining & Fluent APIs

Ved at returnere objektet selv (eller en transformeret udgave) kan operationer kædes sammen i en letlæselig pipeline:

```csharp
public static class TextExtensions
{
    public static string NormalizeSpaces(this string input)
        => string.Join(' ', input.Split(' ', StringSplitOptions.RemoveEmptyEntries));

    public static string Truncate(this string input, int maxLength)
        => input.Length <= maxLength ? input : input[..maxLength] + "...";
}
```

---

```csharp
// Fluent chaining:
string rawInput = "   Dette   er    en   lang   tekstbesked   ";

string processed = rawInput
    .Trim()
    .NormalizeSpaces()
    .Truncate(20);

// Resultat: "Dette er en lang..."
```

---

## 10. Praktiske Eksempler

### 1. Dato-beregninger (`DateTime`)
```csharp
public static class DateTimeExtensions
{
    public static bool IsWeekend(this DateTime date) =>
        date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

    public static DateTime NextWorkday(this DateTime date) =>
        date.DayOfWeek switch
        {
            DayOfWeek.Friday   => date.AddDays(3),
            DayOfWeek.Saturday => date.AddDays(2),
            _                  => date.AddDays(1)
        };
}
```
---

### 2. Valuta & Formatering (`decimal`)
```csharp
public static class CurrencyExtensions
{
    public static string ToDkk(this decimal amount) => $"{amount:N2} DKK";
}

decimal price = 1299.50m;
Console.WriteLine(price.ToDkk()); // "1.299,50 DKK"
```

---

## 11. Fremtidens C#: Nye Extension Members

Traditionelt har C# **kun** understøttet extension *metoder*.
I moderne C# (C# 13 / C# 14 roadmap) udvides konceptet til **Extension Members**:

- **Extension Properties**: Egenskaber med `get` og `set`.
- **Extension Indexers**: Indeksering på typer `[index]`.
- **Static Extension Members**: Statiske metoder/felter knyttet direkte til en type (f.eks. `int.CustomParse(...)`).
- **Extension Operators**: Overload af operatorer på eksisterende typer.

---

```csharp
// Eksempel på moderne Extension Block syntaks (C# roadmap / Extension Types):
public implicit extension PersonExtensions for Person
{
    // Extension Property
    public bool IsAdult => this.Age >= 18;

    // Extension Method
    public string FormalGreeting() => $"Hr./Fru {this.LastName}";
}
```

---

## 12. Best Practices (Do's & Don'ts)

### Gør dette (Do's):
- **Brug det til typer, du ikke ejer**: Udvid .NET frameworks typer eller 3.-parts biblioteker.
- **Hold dem små og fokuserede**: Én specifik hjælpefunktion pr. metode.
- **Brug meningsfulde namespaces**: Læg generelle extensions i et dedikeret namespace (f.eks. `MyApp.Extensions`), så de kun importeres, hvor de behøves.
- **Håndter `null` pænt**: Tag stilling til, hvad der skal ske, hvis input er `null`.

---

### Undgå dette (Don'ts):
- **Foruren ikke globale namespaces**: Undgå at lægge extensions i `System` uden grund (forurener IntelliSense for alle filer).
- **Misbrug det ikke på egne klasser**: Hvis du selv ejer klassen, hører metoden ofte hjemme inde i klassen.
- **Skjul ikke tunge beregninger**: En extension method bør ikke udføre skjulte, langsomme databasekald uden at det fremgår tydeligt af navnet.

---

## 13. Opsummering

- **Extension Methods** lader dig tilføje nye metoder til eksisterende typer uden at ændre eller nedarve fra dem.
- Oprettes som **`static` metoder i en `static` klasse** med **`this`** foran første parameter.
- **LINQ** er det mest udbredte eksempel på extension methods i .NET.
- C#-compileren omskriver instanskaldet til et statisk metodekald bag kulisserne.
- **Instansmetoder har altid forrang** over extension methods.
- Kan kaldes på `null`-værdier (husk `null`-tjek i metoden).
- **Extension Members** i nyere C# udvider konceptet til properties, indexers og statiske medlemmer.
