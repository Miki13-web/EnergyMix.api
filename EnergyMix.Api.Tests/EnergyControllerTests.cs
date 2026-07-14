using EnergyMix.Api.Controllers;
using EnergyMix.Api.Models.Dto;
using EnergyMix.Api.Services;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace EnergyMix.Api.Tests;

public class EnergyControllerTests
{
    private readonly IEnergyService _mockEnergyService;
    private readonly EnergyController _sut;

    public EnergyControllerTests()
    {
        _mockEnergyService = Substitute.For<IEnergyService>();
        _sut = new EnergyController(_mockEnergyService);
    }

    [Fact]
    public async Task GetOptimalWindow_ShouldReturnBadRequest_WhenHoursOutOfRange()
    {
        // Act
        // Wywołujemy z wartością 7, podczas gdy dopuszczalny zakres to 1-6
        var result = await _sut.GetOptimalWindow(7, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task GetOptimalWindow_ShouldReturnOkWithData_WhenValidRequest()
    {
        // Arrange
        var expectedDto = new OptimalWindowDto(DateTime.UtcNow, DateTime.UtcNow.AddHours(3), 80m);
        _mockEnergyService.GetOptimalChargingWindowAsync(3, Arg.Any<CancellationToken>())
            .Returns(expectedDto);

        // Act
        var result = await _sut.GetOptimalWindow(3, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<OptimalWindowDto>(okResult.Value);

        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(80m, returnValue.CleanEnergyPercentage);
    }

    [Fact]
    public async Task GetOptimalWindow_ShouldReturnNotFound_WhenServiceReturnsNull()
    {
        // Arrange
        // Symulacja sytuacji, w której zewnętrzne API nie zwróciło wystarczającej liczby danych
        _mockEnergyService.GetOptimalChargingWindowAsync(3, Arg.Any<CancellationToken>())
            .Returns((OptimalWindowDto?)null);

        // Act
        var result = await _sut.GetOptimalWindow(3, CancellationToken.None);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }
}