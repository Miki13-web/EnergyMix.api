using EnergyMix.Api.Models.External;

namespace EnergyMix.Api.Clients;

public interface ICarbonIntensityClient
{
    Task<IReadOnlyList<CarbonIntensityData>> GetGenerationMixAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
}