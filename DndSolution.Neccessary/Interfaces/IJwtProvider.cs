using DndSolution.Application.Models;

namespace DndSolution.Neccessary;

public interface IJwtProvider
{
    public string GenerateToken(User user);
}