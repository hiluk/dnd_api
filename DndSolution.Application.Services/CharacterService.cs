using Data.Abstractions;
using Data.Entities;
using Data.Entities.Entities;
using DndSolution.Application.Abstractions;
using DndSolution.Application.Models;
using DndSolution.Application.Models.Models;
using DndSolution.Application.Services.Mappers;
using Mapster;

namespace DndSolution.Application.Services;

public class CharacterService : ICharacterService
{

    private readonly ICharactersRepository _repository;

    public CharacterService(ICharactersRepository repository)
    {
        _repository = repository;
    }

    public async Task CreateCharacterAsync(Character character, CancellationToken token)
    {
        // var entity = character.Adapt<CharacterEntity>(); TODO
        
        var entity = CharacterMapper.MapToEntity(character);
        await _repository.SaveCharacterAsync(entity, token);
    }
    
}