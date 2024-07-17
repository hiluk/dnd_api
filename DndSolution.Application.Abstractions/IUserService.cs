using DndSolution.Application.Models;

namespace DndSolution.Application.Abstractions;

public interface IUserService
{
    public Task<string> Register(string userName, string email, string password, CancellationToken token);
    
    public Task<string> Login(string email, string password, CancellationToken token);
}