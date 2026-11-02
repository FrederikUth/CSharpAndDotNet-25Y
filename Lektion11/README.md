# Lektion 11 – SubTrack

Semesteropgaven er besvaret med **Case A: SubTrack**, en full-stack app til at holde styr på abonnementer og faste udgifter.

## Forudsætninger

- .NET 8 SDK
- Docker Desktop eller Podman kørende til PostgreSQL og pgAdmin

## Kør løsningen

Fra repoets rod:

```powershell
dotnet run --project Lektion11\SubTrack.AppHost\SubTrack.AppHost.csproj
```

Aspire starter PostgreSQL med et persistent data volume, API'et og Blazor WebAssembly-klienten. Databasen oprettes og seedes automatisk via EF Core-migrationen ved API-opstart. Aspire Dashboard viser ressourcer, logs og traces.

## Projekter

- `SubTrack.AppHost` – Aspire-orkestrering af PostgreSQL, API og webfrontend.
- `SubTrack.ServiceDefaults` – OpenTelemetry traces/metrics, OTLP-eksport og struktureret JSON-logning.
- `SubTrack.Api` – Controller-baseret REST API, DTO-validering, EF Core/Npgsql og database-migration.
- `SubTrack.Web\Client` – Blazor WebAssembly-dashboard og API-service.
- `SubTrack.Web\Server` – Serverer WebAssembly-filer og proxyer `/api` til API'et via Aspire service discovery; browseren behøver derfor ingen hardcodet API-port.

## Funktioner

- CRUD for abonnementer med pris, valuta, betalingsinterval, datoer, status, kategori og betalingsmetode.
- Kategorier og betalingsmetoder kan oprettes fra brugerfladen.
- Dashboard med normaliseret månedligt forbrug, forventet årsforbrug og fordeling pr. kategori.
- Fremhævelse af aktive abonnementer med fornyelse inden for syv dage.
- Seed-data til kategorier, betalingsmetoder og abonnementer.

API'et returnerer DTO'er og anvender REST-statuskoder (`201 Created`, `204 No Content`, `400 Bad Request` og `404 Not Found`).
