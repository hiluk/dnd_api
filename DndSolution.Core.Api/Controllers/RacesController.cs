using Core.Api.Mappings;
using Core.Sdk.Dtos.Race;
using DndSolution.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Core.Api.Controllers;


[ApiController]
[Route("races")]
public class RacesController : ControllerBase
{
    private readonly IRaceService _service;


    /// <summary>
    /// Работа с расами
    /// </summary>
    /// <param name="service"></param>
    public RacesController(IRaceService service)
    {
        _service = service;
    }


    /// Сохранить расу
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="token"></param>
    [HttpPost("save-new", Name = "Сохранить новую расу")]
    public async Task<IActionResult> SaveRace(RaceDto dto, CancellationToken token)
    {
        try
        {
            var model = RaceModelMapper.MapToModel(dto);
            await _service.SaveRaceAsync(model, token);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return NotFound();
        }
    }
    

    /// Получить все расы
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpGet("", Name = "Получить все расы")]
    [ProducesResponseType<List<RaceDto>>(200)]
    [ProducesResponseType<string>(404)]

    public async Task<IActionResult> GetAllRacesAsync(CancellationToken token)
    {
        try
        {
            var races = await _service.GetAllRacesAsync(token);
            var dtos = races.Select(x => RaceModelMapper.MapToDto(x)).ToList();
            
            if (dtos.IsNullOrEmpty()) return NoContent();

            return Ok(dtos);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        
    }
}