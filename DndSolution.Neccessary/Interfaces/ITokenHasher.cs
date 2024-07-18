using DndSolution.Application.Models;

namespace DndSolution.Neccessary;

public interface ITokenHasher
{
    public string Generate(string token);
    
    public bool Verify(string token, User user);
}