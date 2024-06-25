using DndSolution.Application.Models;
using DndSolution.Application.Models.Models;

namespace DndSolution.Application.Abstractions;

public interface ICharacterService
{
    public Task CreateCharacterAsync(CharacterFull character, CancellationToken token);
}