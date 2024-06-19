using Data.Entities;
using DndSolution.Application.Models;

namespace Data.Abstractions;

public interface ICharacterRepository

{
    public Task SaveCharacterToDb(CharacterEntity character);
}