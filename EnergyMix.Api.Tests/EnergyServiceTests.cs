using EnergyMix.Api.Clients;
using EnergyMix.Api.Models.External;
using EnergyMix.Api.Services;
using NSubstitute;

namespace EnergyMix.Api.Tests;

public class EnergyServiceTests
{
    private readonly ICarbonIntensityClient _mockClient;
    private readonly EnergyService _sut; 

    public EnergyServiceTests()
    {
        _mockClient = Substitute.For<ICarbonIntensityClient>();
        _sut = new EnergyService(_mockClient);
    }

    [Fact]
    public async Task GetOptimalChargingWindowAsync_ShouldReturnBestWindow_WhenDataIsAvailable()
    {
        // Arrange
        var now = DateTime.UtcNow;

        // 4 intervals 30 minutes each
        var mockData = new List<CarbonIntensityData>
        {
            CreateMockData(now, now.AddMinutes(30), "solar", 10), 
            CreateMockData(now.AddMinutes(30), now.AddMinutes(60), "wind", 20), 
            
            CreateMockData(now.AddMinutes(60), now.AddMinutes(90), "hydro", 80), 
            CreateMockData(now.AddMinutes(90), now.AddMinutes(120), "nuclear", 90) 
        };

        _mockClient.GetGenerationMixAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(mockData);

        // Act
      
        var result = await _sut.GetOptimalChargingWindowAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(85m, result.CleanEnergyPercentage);
        Assert.Equal(now.AddMinutes(60), result.StartTime);
        Assert.Equal(now.AddMinutes(120), result.EndTime);
    }

    [Fact]
    public async Task GetOptimalChargingWindowAsync_ShouldReturnNull_WhenNotEnoughData()
    {
        // Arrange
        var mockData = new List<CarbonIntensityData>
        {
            CreateMockData(DateTime.UtcNow, DateTime.UtcNow.AddMinutes(30), "solar", 50)
        };

        _mockClient.GetGenerationMixAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(mockData);

        // Act
        var result = await _sut.GetOptimalChargingWindowAsync(1);

        // Assert
        Assert.Null(result);
    }

    //test data generating
    private static CarbonIntensityData CreateMockData(DateTime from, DateTime to, string fuel, decimal perc)
    {
        return new CarbonIntensityData(
            from,
            to,
            new List<FuelMix> { new FuelMix(fuel, perc), new FuelMix("gas", 100 - perc) }
        );
    }
}