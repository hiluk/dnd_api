using Core.Sdk;
using Core.Sdk.Dtos;
using DndSolution.Application.Abstractions;
using DndSolution.Application.Models;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Core.Api.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
public class CharacterController : ControllerBase
{
    private readonly ICharactersService _service;

    public CharacterController(ICharactersService service)
    {
        _service = service;
    }

    [HttpPost("create")]
    public async Task CreateCharacter(CharacterDto dto)
    {
        var character = dto.Adapt<Character>();
        await _service.CreateCharacter(character);
    }
}