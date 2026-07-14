using System.Text.Json.Serialization;

namespace EnergyMix.Api.Models.External;

public record CarbonIntensityResponse(
    [property: JsonPropertyName("data")] IReadOnlyList<CarbonIntensityData> Data
);

public record CarbonIntensityData(
    [property: JsonPropertyName("from")] DateTime From,
    [property: JsonPropertyName("to")] DateTime To,
    [property: JsonPropertyName("generationmix")] IReadOnlyList<FuelMix> GenerationMix
);

public record FuelMix(
    [property: JsonPropertyName("fuel")] string Fuel,
    [property: JsonPropertyName("perc")] decimal Perc
);