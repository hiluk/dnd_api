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

    public RaceController(IRaceService service)
    {
        _service = service;
    }

    [HttpPost("saveRace")]
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
}