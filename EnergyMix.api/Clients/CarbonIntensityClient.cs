using System.Text.Json;
using EnergyMix.Api.Models.External;

namespace EnergyMix.Api.Clients;

public class CarbonIntensityClient(HttpClient httpClient) : ICarbonIntensityClient
{
    public async Task<IReadOnlyList<CarbonIntensityData>> GetGenerationMixAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var fromStr = from.ToString("yyyy-MM-ddTHH:mmZ");
        var toStr = to.ToString("yyyy-MM-ddTHH:mmZ");

        var url = $"generation/{fromStr}/{toStr}";

        var response = await httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStreamAsync(cancellationToken);
        var result = await JsonSerializer.DeserializeAsync<CarbonIntensityResponse>(content, cancellationToken: cancellationToken);

        return result?.Data ?? Array.Empty<CarbonIntensityData>();
    }
}