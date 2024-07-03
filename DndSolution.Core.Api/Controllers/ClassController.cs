using Core.Api.Mappings;
using Core.Sdk.Dtos.Classes;
using DndSolution.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Core.Api.Controllers;

[ApiController]
[Route("api/v1/class")]
public class ClassController : ControllerBase
{
    private readonly IClassService _service;

    public ClassController(IClassService service)
    {
        _service = service;
    }
    
    [HttpPost("SaveClass")]
    public async Task SaveClassAsync(CharacterClassDto dto, CancellationToken token)
    {
        try
        {
            var model = ClassModelMapper.MapToModel(dto);
            
            await _service.SaveClassAsync(model, token);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }
    
    [HttpGet("GetAllClasses")]
    public async Task<List<CharacterClassDto>> GetAllClassesAsync(CancellationToken token)
    {
        List<CharacterClassDto> classDtos = [];
        var models = await _service.GetAllClassesAsync(token);

        foreach (var characterClass in models)
        {
            classDtos.Add(ClassModelMapper.MapToDto(characterClass));
        }

        return classDtos;
    }
}