using System.Security.Claims;
using Core.Api.Mappings;
using Core.Sdk.Dtos.Characters;
using DndSolution.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Api.Controllers;

/// <summary>
/// Броски характеристик
/// </summary>
[ApiController]
[Authorize]
[Route("/abilities")]
public class AbilitiesController : ControllerBase
{
    private readonly IAbilityRollService _rolls;

    /// <summary>
    /// .ctor
    /// </summary>
    public AbilitiesController(IAbilityRollService rolls)
    {
        _rolls = rolls;
    }

    /// <summary>
    /// Бросить 4к6 без наименьшего для шести характеристик.
    /// Перебрасывать можно сколько угодно; подпись результата действует час.
    /// </summary>
    [HttpPost("roll")]
    [ProducesResponseType<AbilityRollDto>(StatusCodes.Status200OK)]
    public IActionResult Roll()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var roll = _rolls.Roll(userId);

        return Ok(roll.ToDto());
    }
}
