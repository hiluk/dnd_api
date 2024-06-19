using Data.Abstractions;
using Data.Entities;
using DndSolution.Application.Models;
using Mapster;

namespace Data.Repositories;

public class CharactersRepository : ICharactersRepository
{
    public async Task SaveCharacterToDb(CharacterEntity entity)
    {
    }
}