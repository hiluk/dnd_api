using Data.Entities;
using Data.Entities.Entities;
using DndSolution.Application.Models;

namespace Data.Abstractions;

public interface ICharactersRepository
{
    public Task SaveCharacterAsync(CharacterEntity character, CancellationToken token);
}