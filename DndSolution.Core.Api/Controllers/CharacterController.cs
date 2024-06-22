using Core.Api.Mappings;
using Core.Sdk.Dtos;
using DndSolution.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Core.Api.Controllers;

[ApiController]
[Route("/api/v1/character")]
public class CharacterController : ControllerBase
{
    private readonly ICharacterService _service;
    private readonly ILogger<CharacterController> _logger;

    public CharacterController(ICharacterService service, ILogger<CharacterController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost("createPerson")]
    public async Task CreateCharacter(CharacterSaveRequest request, CancellationToken token)
    {
        try
        {
            var character = CharacterMapper.MapToModel(request);
            await _service.CreateCharacterAsync(character, token);
        }
        catch (Exception e)
        {
            _logger.LogError(e,"Не удалось сохранить персонажа");
            throw;
        }        
    }
}