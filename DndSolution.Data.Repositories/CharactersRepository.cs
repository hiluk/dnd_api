using Data.Abstractions;
using Data.Entities;
using DndSolution.Application.Models;
using Mapster;

namespace Data.Repositories;

public class CharacterRepository : ICharacterRepository
{
    public async Task SaveCharacterToDb(CharacterEntity entity)
    {
    }
}