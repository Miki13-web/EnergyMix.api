# EnergyMix API

Backend do zadania rekrutacyjnego — pokazuje miks energetyczny UK i szuka najlepszego okna do ładowania auta elektrycznego.

Dane biorę z [Carbon Intensity API](https://carbon-intensity.github.io/api-definitions/?shell#get-generation-from-to).

## Uruchomienie

Wymagania: .NET 8 SDK.

```bash
dotnet run --project EnergyMix.api
```

API startuje na `http://localhost:5194`. W trybie Development dostępny jest Swagger pod `/swagger`.

Testy:

```bash
dotnet test
```

## Endpointy

### `GET /api/energy/mix`

Zwraca uśredniony miks na dziś, jutro i pojutrze. W odpowiedzi dla każdego dnia:
- lista źródeł z procentami,
- suma czystej energii (biomass, nuclear, hydro, wind, solar).

### `GET /api/energy/optimal-window?hours={1-6}`

Szuka okna ładowania z najwyższym udziałem czystej energii w najbliższych dwóch dniach.  
Jeden punkt w API to 30 minut, więc np. 3 godziny = 6 interwałów.

Przykład:

```http
GET /api/energy/optimal-window?hours=3
```

## Frontend

CORS jest ustawiony na `http://localhost:5173` (Vite).  
Repozytorium frontu: `energy-mix-frontend` obok tego projektu.

## Docker / Render

W katalogu głównym jest `Dockerfile`. Na Renderze wystarczy Web Service z runtime Docker — port aplikacji to `8080`.

W zmiennych środowiskowych frontu ustaw URL backendu, np. `VITE_API_URL=https://twoj-backend.onrender.com`.
