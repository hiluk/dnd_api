using Data.Abstractions;
using Data.Entities;
using DndSolution.Application.Abstractions;
using Mapster;

namespace DndSolution.Application.Services;

public class CharactersService : ICharactersService
{

    private readonly ICharactersRepository _repository;

    public CharactersService(ICharactersRepository repository)
    {
        _repository = repository;
    }

    public async Task CreateCharacter(ICharacter character)
    {
        var entity = character.Adapt<CharacterEntity>();
        await _repository.SaveCharacterToDb(entity);
    }
    
}