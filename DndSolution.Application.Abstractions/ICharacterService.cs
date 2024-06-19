

namespace DndSolution.Application.Abstractions;

public interface ICharacterService
{
    public Task CreateCharacter(ICharacter dto);
}
