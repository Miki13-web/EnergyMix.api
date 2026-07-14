using EnergyMix.Api.Clients;
using EnergyMix.Api.Domain;
using EnergyMix.Api.Models.Dto;
using EnergyMix.Api.Models.External;

namespace EnergyMix.Api.Services;

public class EnergyService(ICarbonIntensityClient carbonClient) : IEnergyService
{
    public async Task<IReadOnlyList<DailyMixDto>> GetThreeDaysMixAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        var endDate = today.AddDays(3);

        var data = await carbonClient.GetGenerationMixAsync(today, endDate, cancellationToken);

        return data
            .Where(d => d.From >= today && d.From < endDate)
            .GroupBy(d => d.From.Date)
            .Select(group =>
            {
                var fuels = group
                    .SelectMany(interval => interval.GenerationMix)
                    .GroupBy(f => f.Fuel)
                    .Select(fg => new FuelMixDto(fg.Key, Math.Round(fg.Average(x => x.Perc), 2)))
                    .ToList();

                var cleanEnergy = fuels
                    .Where(f => EnergyConstants.IsCleanEnergy(f.Fuel))
                    .Sum(f => f.Percentage);

                return new DailyMixDto(group.Key, fuels, cleanEnergy);
            })
            .OrderBy(d => d.Date)
            .ToList();
    }

    public async Task<OptimalWindowDto?> GetOptimalChargingWindowAsync(int hours, CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        var endDate = today.AddDays(2);

        var data = await carbonClient.GetGenerationMixAsync(today, endDate, cancellationToken);
        var windowSize = hours * 2;

        if (data.Count < windowSize)
            return null;

        var bestStart = 0;
        var bestScore = decimal.MinValue;

        for (var i = 0; i <= data.Count - windowSize; i++)
        {
            var score = data
                .Skip(i)
                .Take(windowSize)
                .Average(interval => interval.GenerationMix
                    .Where(f => EnergyConstants.IsCleanEnergy(f.Fuel))
                    .Sum(f => f.Perc));

            if (score > bestScore)
            {
                bestScore = score;
                bestStart = i;
            }
        }

        var start = data[bestStart];
        var end = data[bestStart + windowSize - 1];

        return new OptimalWindowDto(start.From, end.To, Math.Round(bestScore, 2));
    }
}
