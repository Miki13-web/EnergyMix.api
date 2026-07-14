namespace EnergyMix.Api.Models.Dto;

public record OptimalWindowDto(
    DateTime StartTime,
    DateTime EndTime,
    decimal CleanEnergyPercentage
);