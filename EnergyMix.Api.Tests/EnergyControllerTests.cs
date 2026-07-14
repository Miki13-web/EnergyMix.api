using EnergyMix.Api.Controllers;
using EnergyMix.Api.Models.Dto;
using EnergyMix.Api.Services;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace EnergyMix.Api.Tests;

public class EnergyControllerTests
{
    private readonly IEnergyService _energyService = Substitute.For<IEnergyService>();
    private readonly EnergyController _controller;

    public EnergyControllerTests()
    {
        _controller = new EnergyController(_energyService);
    }

    [Fact]
    public async Task GetOptimalWindow_zwraca_400_gdy_hours_poza_zakresem()
    {
        var result = await _controller.GetOptimalWindow(7, CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }

    [Fact]
    public async Task GetOptimalWindow_zwraca_okno_gdy_dane_sa_dostepne()
    {
        var expected = new OptimalWindowDto(DateTime.UtcNow, DateTime.UtcNow.AddHours(3), 80m);
        _energyService.GetOptimalChargingWindowAsync(3, Arg.Any<CancellationToken>())
            .Returns(expected);

        var result = await _controller.GetOptimalWindow(3, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        var window = Assert.IsType<OptimalWindowDto>(ok.Value);
        Assert.Equal(80m, window.CleanEnergyPercentage);
    }

    [Fact]
    public async Task GetOptimalWindow_zwraca_404_gdy_brak_danych()
    {
        _energyService.GetOptimalChargingWindowAsync(3, Arg.Any<CancellationToken>())
            .Returns((OptimalWindowDto?)null);

        var result = await _controller.GetOptimalWindow(3, CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}
