using Data.Entities;
using DndSolution.Application.Models;

namespace Data.Abstractions;

public interface ICharactersRepository

{
    public Task SaveCharacterToDb(CharacterEntity character);
}