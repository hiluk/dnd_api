using Core.Api.Mappings;
using Core.Sdk.Dtos.Classes;
using DndSolution.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Core.Api.Controllers;

[ApiController]
[Route("classes", Name = "Classes") ]
public class ClassesController : ControllerBase
{
    private readonly IClassService _service;

    public ClassesController(IClassService service)
    {
        _service = service;
    }
    
    [HttpPost("save-new", Name = "Сохранить класс")]
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
    
    [HttpGet("", Name = "Получить все классы")]
    [ProducesResponseType<List<CharacterClassDto>>(200)]
    [ProducesResponseType<string>(404)]
    public async Task<IActionResult> GetAllClassesAsync(CancellationToken token)
   {
       try
       {
           List<CharacterClassDto> classDtos = [];
           var models = await _service.GetAllClassesAsync(token);

           foreach (var characterClass in models)
           {
               classDtos.Add(ClassModelMapper.MapToDto(characterClass));
           }

           if (classDtos.IsNullOrEmpty()) return NoContent();

           return Ok(classDtos);
       }
       catch (Exception e)
       {
           return BadRequest(e.Message);
       }
   }
}