# EnergyMix API

Backend do zadania rekrutacyjnego — pokazuje miks energetyczny UK i szuka najlepszego okna do ładowania auta elektrycznego.

Dane biorę z [Carbon Intensity API](https://carbon-intensity.github.io/api-definitions/?shell#get-generation-from-to).

## Funkcjonalności

- Pobieranie danych o generacji energii na najbliższe trzy dni.
- Obliczanie średniego dziennego udziału poszczególnych źródeł energii.
- Algorytm (sliding window) wyszukujący optymalne okno czasowe (1–6 godzin) do ładowania pojazdów elektrycznych na podstawie maksymalnego procentowego udziału czystej energii.
- Definicja czystej energii obejmuje: biomasę, energię nuklearną, wodną, wiatrową oraz słoneczną.

## Technologie

- **.NET 8** (ASP.NET Core Web API)
- **C# 12**
- **xUnit** + **NSubstitute** (testy jednostkowe)
- **Docker** (wieloetapowy Dockerfile pod deployment)

## Uruchomienie lokalne

Wymagane zainstalowane środowisko **.NET 8 SDK**.

### 1. Uruchomienie aplikacji

```bash
dotnet run --project EnergyMix.api
```

### 2. Adres lokalny

Aplikacja domyślnie nasłuchuje pod adresem:

```
http://localhost:5194
```

### 3. Przykładowe wywołania

Pobranie miksu energetycznego:

```bash
curl "http://localhost:5194/api/Energy/mix"
```

Wyznaczenie optymalnego okna ładowania (3 godziny):

```bash
curl "http://localhost:5194/api/Energy/optimal-window?hours=3"
```

## Endpointy

| Metoda | Ścieżka | Opis |
| --- | --- | --- |
| GET | `/api/Energy/mix` | Zwraca miks energetyczny dla trzech najbliższych dni |
| GET | `/api/Energy/optimal-window?hours={1-6}` | Zwraca optymalne okno ładowania dla podanej liczby godzin |

### Przykładowa odpowiedź — `/mix`

```json
[
  {
    "date": "2026-07-14T00:00:00Z",
    "mix": [
      { "fuel": "wind", "percentage": 25.64 },
      { "fuel": "gas", "percentage": 24.27 }
    ],
    "cleanEnergyPercentage": 59.94
  }
]
```

### Przykładowa odpowiedź — `/optimal-window`

```json
{
  "startTime": "2026-07-14T11:30:00Z",
  "endTime": "2026-07-14T14:30:00Z",
  "cleanEnergyPercentage": 74.88
}
```

## Testy

Uruchomienie testów jednostkowych:

```bash
dotnet test
```

## Docker

Zbudowanie obrazu:

```bash
docker build -t energymix-api .
```

Uruchomienie kontenera:

```bash
docker run -p 5194:8080 energymix-api
```

## Deployment

Aplikacja wdrożona na Render:

```
https://energymix-api-5v57.onrender.com/api/Energy
```

## Frontend

Repozytorium frontendu (React + TypeScript) https://github.com/Miki13-web/energy-mix-frontend - korzysta z powyższego API do wizualizacji wykresów i formularza wyboru optymalnego okna ładowania.

