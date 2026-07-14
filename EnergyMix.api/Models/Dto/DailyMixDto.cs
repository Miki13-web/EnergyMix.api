namespace EnergyMix.Api.Models.Dto;

public record DailyMixDto(
    DateTime Date,
    IReadOnlyList<FuelMixDto> Mix,
    decimal CleanEnergyPercentage
);