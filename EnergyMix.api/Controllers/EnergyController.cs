using Microsoft.AspNetCore.Mvc;
using EnergyMix.Api.Services;

namespace EnergyMix.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnergyController(IEnergyService energyService) : ControllerBase
{
    [HttpGet("mix")]
    public async Task<IActionResult> GetThreeDaysMix(CancellationToken cancellationToken)
    {
        var result = await energyService.GetThreeDaysMixAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("optimal-window")]
    public async Task<IActionResult> GetOptimalWindow([FromQuery] int hours, CancellationToken cancellationToken)
    {
        if (hours is < 1 or > 6)
            return BadRequest(new { error = "Parametr hours musi być z zakresu 1-6." });

        var result = await energyService.GetOptimalChargingWindowAsync(hours, cancellationToken);
        if (result is null)
            return NotFound(new { error = "Brak wystarczających danych prognozy." });

        return Ok(result);
    }
}
