

namespace DndSolution.Application.Abstractions;

public interface ICharactersService
{
    public Task CreateCharacter(ICharacter character);
    
}