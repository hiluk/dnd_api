using DndSolution.Application.Models;

namespace DndSolution.Application.Abstractions;

public interface IAuthService
{
    public Task<TokensModel> Register(string userName, string email, string password, CancellationToken token);
    
    public Task<TokensModel> Login(string email, string password, CancellationToken token);

    public Task<TokensModel> Refresh(string refreshToken, string email, CancellationToken token);
}