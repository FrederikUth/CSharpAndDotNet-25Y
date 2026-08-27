# Opgaver: Lektion 02

### Opgave 1: Udskriv og filtrer data

Brug Linq til at løse nedenstående opgaver.

1. Udskriv `FullName` og `HogwartsHouse` for alle karaktererne i konsollen.
2. Udskriv alle karakterer, der tilhører kollegiet **Gryffindor**.
3. Udskriv navnene på de karakterer, der har børn (`Children.Count > 0`), samt børnenes navne.

### Opgave 2: Filtrering og sortering i webshop-data

Brug Linq til at løse nedenstående opgaver ud fra `SeedData` i projektet `Opgave02`.

1. Find alle produkter i kategorien `Category.Elektronik`, som er på lager (`StockCount > 0`).
2. Udskriv navn og pris for disse produkter, sorteret efter pris i faldende rækkefølge (dyreste først).
3. Find alle kunder fra byen **"Aarhus"** og udskriv deres navne.
4. **Bonus:** Find de 3 mest solgte produkter målt på samlet solgt antal.

> [!TIP]
> I får brug for SelectMany, GroupBy, Sum, OrderByDescending og Take metoderne.

5. **Bonus:** Lav en opgørelse over alle kunder og deres samlede købsbeløb i shoppen.
   1. For hver kunde beregnes den samlede omsætning: ∑(Quantity × Price).
   2. Projicer til en anonym type: { CustomerName, OrderCount, TotalSpent }.
   3. Inkluder også kunder, der har 0 ordrer (f.eks. Clara Møller – skal have TotalSpent = 0).
   4. Sorter kunderne, så den med højst forbrug kommer først.
      • Fokus-metoder: Select, SelectMany, DefaultIfEmpty, Sum, OrderByDescending.
---

### Opgave 3: Extension Method – Simpel Leetspeak

I denne opgave skal du lave en **Extension Method** til `string`, som konverterer en tekststreng til simpel [Leetspeak](https://da.wikipedia.org/wiki/Leetspeak).

1. Opret en statisk klasse (f.eks. `StringExtensions`).
2. Opret en extension method `ToLeetSpeak()` (eller `ToLeet()`), der gennemløber strengen og erstatter udvalgte bogstaver med tal/symboler.
    - En simpel udgave kan f.eks. erstatte:
        - `a` / `A` $\rightarrow$ `4`
        - `e` / `E` $\rightarrow$ `3`
        - `i` / `I` / `l` / `L` $\rightarrow$ `1`
        - `o` / `O` $\rightarrow$ `0`
        - `s` / `S` $\rightarrow$ `5`
        - `t` / `T` $\rightarrow$ `7`
3. Afprøv din extension method i `Program.cs`:
   ```csharp
   string text = "Dette er et leetspeak eksempel";
   Console.WriteLine(text.ToLeetSpeak());
   // Output f.eks.: "D3773 3r 37 l3375p34k 3k53mp3l"
   ```

> [!TIP]
> **Brug `StringBuilder` frem for string concatenation (`+` / `+=`):**
> I C# er `string` **uforanderlig (immutable)**. Det betyder, at hver gang du laver `result += newChar` i en løkke, oprettes der et helt nyt `string`-objekt i hukommelsen, og den gamle streng skal efterfølgende ryddes op af Garbage Collector. Dette er ineffektivt ($O(N^2)$ tidskompleksitet ved mange tegn).
>
> Brug i stedet `System.Text.StringBuilder`, som benytter en intern buffer og muterer teksten direkte (`sb.Append(...)`), hvilket giver markant bedre performance ($O(N)$) og lavere hukommelsesforbrug.

---

