using Core.Api.Mappings;
using Core.Sdk.Dtos.Race;
using DndSolution.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Core.Api.Controllers;


[ApiController]
[Route("api/v1/race")]
public class RaceController : ControllerBase
{
    private readonly IRaceService _service;


    /// <summary>
    /// Работа с расами
    /// </summary>
    /// <param name="service"></param>
    public RaceController(IRaceService service)
    {
        _service = service;
    }

    /// <summary>
    /// Сохраниние новой расы
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="token"></param>
    [HttpPost("SaveRace")]
    public async Task SaveRace(RaceDto dto, CancellationToken token)
    {
        try
        {
            var model = RaceModelMapper.MapToModel(dto);
            await _service.SaveRaceAsync(model, token);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    /// <summary>
    /// Получение всех рас
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpGet("GetAllRaces")]
    public async Task<List<RaceDto>> GetAllRacesAsync(CancellationToken token)
    {
        var races = await _service.GetAllRacesAsync(token);
        List<RaceDto> racesDto = [];

        foreach (var race in races)
        {
            racesDto.Add(RaceModelMapper.MapToDto(race));
        }

        return racesDto;
    }
}