using Microsoft.AspNetCore.Mvc;
using EnergyMix.Api.Services;
using EnergyMix.Api.Models.Dto;

namespace EnergyMix.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnergyController(IEnergyService energyService) : ControllerBase
{
    [HttpGet("mix")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<DailyMixDto>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetThreeDaysMix(CancellationToken cancellationToken)
    {
        try
        {
            var result = await energyService.GetThreeDaysMixAsync(cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "An error occurred while fetching energy mix data.",
                details = ex.Message
            });
        }
    }

    [HttpGet("optimal-window")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OptimalWindowDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOptimalWindow([FromQuery] int hours, CancellationToken cancellationToken)
    {
        if (hours < 1 || hours > 6)
        {
            return BadRequest(new { message = "Charging window must be between 1 and 6 hours." });
        }

        try
        {
            var result = await energyService.GetOptimalChargingWindowAsync(hours, cancellationToken);
            if (result == null)
            {
                return NotFound(new { message = "Could not calculate optimal window. External data might be unavailable." });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "An error occurred while calculating the optimal window.",
                details = ex.Message
            });
        }
    }
}