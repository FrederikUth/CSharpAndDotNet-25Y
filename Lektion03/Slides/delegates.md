---
marp: true
theme: default
paginate: true
header: 'Delegates i C#'
footer: 'C# & .NET'
---

# Delegates i C#
### Fra Java Functional Interfaces (`Function`, `Consumer`) til C# Delegates (`Func`, `Action`) og Events

---

## Indholdsfortegnelse

1. **Broen fra Java til C#**
2. **Hvad er en Delegate i C#?**
3. **Erklæring og Anvendelse af Custom Delegates**
4. **Indbyggede Delegates: `Func<T>`, `Action<T>` og `Predicate<T>`**
5. **Sammenligningstabel: Java vs. C#**
6. **Lambda-udtryk & Method Groups**
7. **Multicast Delegates (`+=` og `-=`)**
8. **Praktiske Eksempler (Callbacks & LINQ)**
9. **Opsummering**

---

## 1. Broen fra Java til C#

I **Java** kender I funktionel programmering gennem **Functional Interfaces** (SAM - *Single Abstract Method* interfaces):

- `@FunctionalInterface` med ét abstrakt metodemønster (f.eks. `Function<T, R>`, `Consumer<T>`, `Predicate<T>`).
- En Java lambda eller metodereference opretter et objekteksemplar, der implementerer interfacet.

I **C#** er funktioner **førsteklasses borgere** via **Delegates**:

- En `delegate` er en **typesikker reference til en eller flere metoder**.
- Man behøver ikke definere et interface for at sende metoder som argumenter!
- C# har både sit eget `delegate`-nøgleord og indbyggede generiske typeskabeloner (`Func` og `Action`).

---

## 2. Hvad er en Delegate i C#?

- En **delegate** er en reference-type, der repræsenterer metoder med en bestemt **metodesignatur** (returtype og parameter-typer).
- Metoden, der refereres til, kan være:
  - En **statisk metode** (`static void Log(string msg)`)
  - En **instansmetode** (`obj.Process(string msg)`)
  - Et **lambda-udtryk** (`msg => Console.WriteLine(msg)`)

### Sammenligning af mental model:
- **Java**: *"Her er et objekt, der implementerer et interface med metoden `apply()`/`accept()`."*
- **C#**: *"Her er en direkte reference til en metode, som du kan kalde som en funktion."*

---

## 3. Erklæring af Custom Delegates

I C# kan man definere sin egen delegate-type med nøgleordet `delegate`:

### C# Syntaks:
```csharp
// 1. Deklarer en delegate-type (definerer signaturen: tager string, returnerer void)
public delegate void LogHandler(string message);

class Program
{
    // 2. En metode der matcher signaturen
    static void WriteToConsole(string text)
    {
        Console.WriteLine($"[LOG]: {text}");
    }
    
```
---

```csharp
        
    static void Main()
    {
        // 3. Tildel metoden til delegate-variablen
        LogHandler logger = WriteToConsole;

        // 4. Eksekver delegaten (kalder WriteToConsole direkte uden .accept() / .apply())
        logger("Hej fra C# delegates!"); 
    }
}
```

---

## Java-modstykke til Custom Delegate

Hvis man skulle skrive det samme i Java, kræver det et interface:

### Java (SAM Interface):
```java
// Java kræver et interface med @FunctionalInterface
@FunctionalInterface
public interface LogHandler {
    void log(String message);
}

public class Main {
    static void writeToConsole(String text) {
        System.out.println("[LOG]: " + text);
    }
```

---

```java
    public static void main(String[] args) {
        LogHandler logger = Main::writeToConsole;
        logger.log("Hej fra Java!"); // Kræver eksplicit metodekald .log()
    }
}
```

*I C# kaldes delegaten direkte som en funktion: `logger("besked")` i stedet for `logger.log("besked")`.*

---

## 4. Indbyggede Delegates: `Func`, `Action` og `Predicate`

Ligesom Java har `java.util.function`-pakken (`Function`, `Consumer`, etc.), har C# indbyggede generiske delegates i `System`-namespacet:

### 1. `Action<...>` (Svarer til Java `Consumer<T>` / `Runnable`)
- Bruges til metoder, der **returnerer `void`**.
- `Action` (0 parametre, returnerer `void`)
- `Action<T>` (1 parameter af type `T`, returnerer `void`)
- `Action<T1, T2>` (2 parametre, returnerer `void`)

---

## Indbyggede Delegates (fortsat): `Func<...>`

### 2. `Func<..., TResult>` (Svarer til Java `Function<T, R>` / `Supplier<T>`)
- Bruges til metoder, der **returnerer en værdi**.
- **Sidste typeparameter** er altid **returtypen**!
- `Func<TResult>` (0 parametre, returnerer `TResult` — svarer til Java `Supplier<T>`)
- `Func<T, TResult>` (1 parameter `T`, returnerer `TResult` — svarer til Java `Function<T, R>`)
- `Func<T1, T2, TResult>` (2 parametre, returnerer `TResult` — svarer til Java `BiFunction<T, U, R>`)

---

## Indbyggede Delegates (fortsat): `Predicate<T>`

### 3. `Predicate<T>` (Svarer til Java `Predicate<T>`)
- Bruges til metoder, der tager 1 parameter `T` og returnerer en `bool`.
- Er i praksis en alias/synonym for `Func<T, bool>`.

---

### Eksempel i C#:
```csharp
// Action: Returnerer void
Action<string> print = msg => Console.WriteLine(msg);
print("Hello Action!");

// Func: Tager int, returnerer string (Sidste parameter er returtypen!)
Func<int, string> formatNumber = num => $"Tallet er {num}";
string result = formatNumber(42);

// Predicate: Tager int, returnerer bool
Predicate<int> isEven = num => num % 2 == 0;
bool check = isEven(4); // true
```

*Bemærk: I 95% af tilfældene i moderne C# bruger man `Func<...>` og `Action<...>` i stedet for at oprette egne custom `delegate`-typer!*

---

## 5. Sammenligningstabel: Java vs. C#

| Koncept | Java | C# |
| :--- | :--- | :--- |
| **Syntaks for funktionstype** | `@FunctionalInterface` (Interface) | `delegate` (Førsteklasses type) |
| **Metode uden returværdi (`void`)** | `Consumer<T>`, `BiConsumer<T1,T2>`, `Runnable` | `Action<T>`, `Action<T1,T2>`, `Action` |
| **Metode med returværdi** | `Function<T, R>`, `BiFunction<T1,T2, R>` | `Func<T, R>`, `Func<T1, T2, R>` |
| **Værdi-leverandør (0 parametre)** | `Supplier<T>` | `Func<T>` |

---

## 5. Sammenligningstabel: Java vs. C# fortsat

| Koncept | Java | C# |
| :--- | :--- | :--- |
| **Betingelse (returnerer boolean)** | `Predicate<T>` | `Predicate<T>` (eller `Func<T, bool>`) |
| **Metodeafvikling / Kald** | `fn.apply(x)` / `consumer.accept(x)` | `fn(x)` (eller `fn.Invoke(x)`) |
| **Metodereferencer** | `Class::staticMethod`, `obj::instanceMethod` | `Class.StaticMethod`, `obj.InstanceMethod` |
| **Multi-kald kæde** | `c1.andThen(c2)` | `d1 += d2` (Multicast Delegate) |

---

## 6. Lambda-udtryk & Method Groups

Både Java og C# understøtter lambda-udtryk og metodereferencer.

### Syntaks-sammenligning:

#### Lambda-udtryk:
- **Java**: `(x, y) -> x + y`
- **C#**: `(x, y) => x + y` *(bruger `=>` i stedet for `->`)*

---

#### Metodereference vs. Method Group:
- **Java** bruger `::` operator:
  ```java
  List<String> names = List.of("anna", "bob");
  names.forEach(System.out::println);
  ```
- **C#** bruger **Method Group Conversion** (direkte navne-reference uden `::`):
  ```csharp
  List<string> names = new() { "anna", "bob" };
  names.ForEach(Console.WriteLine); // Metodenavnet videregives direkte
  ```

---

## Method Group Conversion i C#

I C# behøver du hverken `::` eller et lambda-udtryk, hvis metodens signatur matcher delegaten:

```csharp
class Calculator
{
    public static int Double(int number) => number * 2;
    public bool IsPositive(int number) => number > 0;
}

// Eksempel på brug:
Calculator calc = new();

// Metodereference til statisk metode
Func<int, int> op1 = Calculator.Double;

// Metodereference til instansmetode (binder både metoden OG 'calc' instansen)
Predicate<int> op2 = calc.IsPositive;

```

---

```csharp

int res1 = op1(5);       // 10
bool res2 = op2(10);     // true
```

---

## 7. Multicast Delegates (`+=` og `-=`)

En af de **største forskelle** fra Java: En C# delegate kan holde på **flere metoder samtidig**!
Dette kaldes en **Multicast Delegate**.

```csharp
Action<string> logger = Console.WriteLine;

// Tilføj endnu en metode til delegatens invokeringsliste med +=
logger += LogToFile;
logger += LogToDatabase;

// Kalder ALLE 3 metoder i rækkefølge med den samme argument-værdi!
logger("Bruger logget ind");

// Fjern en metode igen med -=
logger -= LogToDatabase;
```
---

### Hvordan håndteres returværdier i Multicast Delegates?
- Hvis delegaten returnerer en værdi (f.eks. `Func<int>`), vil et multicast-kald eksekvere alle metoder, men kun returnere resultatet fra **den SIDSTE metode** i listen.
- Derfor bruges multicast primært med `void`-metoder (`Action` / hændelser).

---

## 8. Praktiske Eksempler (LINQ & Callbacks)

### Delegates er motoren i C# LINQ
Præcis ligesom Java Streams bruger `Function` og `Predicate`, bruger C# LINQ `Func<T, TResult>` og `Func<T, bool>`:

```csharp
List<int> numbers = new() { 1, 2, 3, 4, 5, 6 };

// LINQ .Where tager Func<int, bool> (eller Predicate<int>)
// LINQ .Select tager Func<int, int> (eller Func<T, R>)
var result = numbers
    .Where(n => n % 2 == 0)      // Func<int, bool>
    .Select(n => n * 10);        // Func<int, int>

foreach (var val in result)
{
    Console.WriteLine(val); // 20, 40, 60
}
```
---

## 9. Opsummering

- **C# Delegates** er typesikre reference-typer til metoder (førsteklasses funktioner).
- **Java vs C# mapping**:
  - `Consumer<T>` $\rightarrow$ `Action<T>`
  - `Function<T, R>` $\rightarrow$ `Func<T, R>`
  - `Supplier<T>` $\rightarrow$ `Func<T>`
  - `Predicate<T>` $\rightarrow$ `Predicate<T>` / `Func<T, bool>`
- **Method Groups**: `obj.Method` i C# svarer til `obj::Method` i Java.
- **Multicast**: En C# delegate kan indeholde en hel kæde af metoder med `+=` og `-=`.
