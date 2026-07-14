using EnergyMix.Api.Clients;
using EnergyMix.Api.Models.External;
using EnergyMix.Api.Services;
using NSubstitute;

namespace EnergyMix.Api.Tests;

public class EnergyServiceTests
{
    private readonly ICarbonIntensityClient _client = Substitute.For<ICarbonIntensityClient>();
    private readonly EnergyService _service;

    public EnergyServiceTests()
    {
        _service = new EnergyService(_client);
    }

    [Fact]
    public async Task GetThreeDaysMix_ignoruje_interwal_przed_poczatkiem_zakresu()
    {
        var day = DateTime.UtcNow.Date;

        var data = new List<CarbonIntensityData>
        {
            Interval(day.AddMinutes(-30), day, "wind", 40),
            Interval(day, day.AddMinutes(30), "solar", 60),
            Interval(day.AddDays(1), day.AddDays(1).AddMinutes(30), "hydro", 70),
            Interval(day.AddDays(2), day.AddDays(2).AddMinutes(30), "nuclear", 80),
        };

        _client.GetGenerationMixAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(data);

        var result = await _service.GetThreeDaysMixAsync();

        Assert.Equal(3, result.Count);
        Assert.Equal(day.Date, result[0].Date);
        Assert.Equal(day.AddDays(1).Date, result[1].Date);
        Assert.Equal(day.AddDays(2).Date, result[2].Date);
    }

    [Fact]
    public async Task GetOptimalChargingWindow_wybiera_okno_z_najwyzszym_udzialem_czystej_energii()
    {
        var now = DateTime.UtcNow;

        var data = new List<CarbonIntensityData>
        {
            Interval(now, now.AddMinutes(30), "solar", 10),
            Interval(now.AddMinutes(30), now.AddMinutes(60), "wind", 20),
            Interval(now.AddMinutes(60), now.AddMinutes(90), "hydro", 80),
            Interval(now.AddMinutes(90), now.AddMinutes(120), "nuclear", 90),
        };

        _client.GetGenerationMixAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(data);

        var result = await _service.GetOptimalChargingWindowAsync(1);

        Assert.NotNull(result);
        Assert.Equal(85m, result.CleanEnergyPercentage);
        Assert.Equal(now.AddMinutes(60), result.StartTime);
        Assert.Equal(now.AddMinutes(120), result.EndTime);
    }

    [Fact]
    public async Task GetOptimalChargingWindow_zwraca_null_gdy_brakuje_interwalow()
    {
        var data = new List<CarbonIntensityData>
        {
            Interval(DateTime.UtcNow, DateTime.UtcNow.AddMinutes(30), "solar", 50),
        };

        _client.GetGenerationMixAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(data);

        var result = await _service.GetOptimalChargingWindowAsync(1);

        Assert.Null(result);
    }

    private static CarbonIntensityData Interval(DateTime from, DateTime to, string cleanFuel, decimal cleanPerc)
    {
        return new CarbonIntensityData(
            from,
            to,
            new List<FuelMix>
            {
                new(cleanFuel, cleanPerc),
                new("gas", 100 - cleanPerc)
            });
    }
}
