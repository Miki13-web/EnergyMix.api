using EnergyMix.Api.Clients;
using EnergyMix.Api.Domain;
using EnergyMix.Api.Models.Dto;
using EnergyMix.Api.Models.External;

namespace EnergyMix.Api.Services;

public class EnergyService(ICarbonIntensityClient carbonClient) : IEnergyService
{
    public async Task<IReadOnlyList<DailyMixDto>> GetThreeDaysMixAsync(CancellationToken cancellationToken = default)
    {
        // Pobieramy dane na dzisiaj, jutro i pojutrze
        var today = DateTime.UtcNow.Date;
        var endDate = today.AddDays(3);

        var data = await carbonClient.GetGenerationMixAsync(today, endDate, cancellationToken);

        // Grupowanie po dacie i wyliczanie średnich
        var dailyMixes = data
            .GroupBy(d => d.From.Date)
            .Select(group =>
            {
                var allFuels = group.SelectMany(g => g.GenerationMix)
                                    .GroupBy(f => f.Fuel)
                                    .Select(fg => new FuelMixDto(
                                        fg.Key,
                                        Math.Round(fg.Average(x => x.Perc), 2)))
                                    .ToList();

                var cleanEnergyTotal = allFuels
                    .Where(f => EnergyConstants.IsCleanEnergy(f.Fuel))
                    .Sum(f => f.Percentage);

                return new DailyMixDto(group.Key, allFuels, cleanEnergyTotal);
            })
            .OrderBy(d => d.Date)
            .ToList();

        return dailyMixes;
    }

    public async Task<OptimalWindowDto?> GetOptimalChargingWindowAsync(int hours, CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        var endDate = today.AddDays(2); // Dane na najbliższe dwa dni dla okna ładowania

        var data = await carbonClient.GetGenerationMixAsync(today, endDate, cancellationToken);

        if (data.Count == 0) return null;

        // 1 godzina to dwa interwały 30-minutowe
        int windowSize = hours * 2;

        if (data.Count < windowSize) return null;

        decimal maxCleanEnergy = -1;
        int bestStartIndex = 0;

        // Algorytm przesuwnego okna
        for (int i = 0; i <= data.Count - windowSize; i++)
        {
            var window = data.Skip(i).Take(windowSize);

            var averageCleanEnergy = window.Average(w =>
                w.GenerationMix
                 .Where(f => EnergyConstants.IsCleanEnergy(f.Fuel))
                 .Sum(f => f.Perc)
            );

            if (averageCleanEnergy > maxCleanEnergy)
            {
                maxCleanEnergy = averageCleanEnergy;
                bestStartIndex = i;
            }
        }

        var bestStartData = data[bestStartIndex];
        var bestEndData = data[bestStartIndex + windowSize - 1];

        return new OptimalWindowDto(
            bestStartData.From,
            bestEndData.To,
            Math.Round(maxCleanEnergy, 2)
        );
    }
}