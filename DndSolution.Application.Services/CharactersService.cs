using System.Web.Mvc;
using Core.Sdk.Dtos;
using Data.Abstractions;
using Data.Entities;
using DndSolution.Application.Abstractions;
using DndSolution.Application.Models;
using Mapster;

namespace DndSolution.Application.Services;

public class CharactersService : ICharactersService
{

    private readonly ICharacterRepository _repository;

    public CharactersService(ICharacterRepository repository)
    {
        _repository = repository;
    }

    public async Task CreateCharacter(ICharacter character)
    {
        var entity = character.Adapt<CharacterEntity>();
        await _repository.SaveCharacterToDb(entity);
    }
    
}