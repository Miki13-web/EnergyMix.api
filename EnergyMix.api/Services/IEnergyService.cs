using EnergyMix.Api.Models.Dto;

namespace EnergyMix.Api.Services;

public interface IEnergyService
{
    Task<IReadOnlyList<DailyMixDto>> GetThreeDaysMixAsync(CancellationToken cancellationToken = default);
    Task<OptimalWindowDto?> GetOptimalChargingWindowAsync(int hours, CancellationToken cancellationToken = default);
}