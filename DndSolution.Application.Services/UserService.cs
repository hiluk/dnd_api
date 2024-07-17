using Data.Abstractions;
using DndSolution.Application.Abstractions;
using DndSolution.Application.Models;
using DndSolution.Neccessary;

namespace DndSolution.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    

    public UserService(IUserRepository repository, IPasswordHasher passwordHasher, IJwtProvider jwtProvider)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
    }

    public async Task<string> Register(string userName, string email, string password, CancellationToken token)
    {
        var hashedPassword = _passwordHasher.Generate(password);
        
        await _repository.SaveUserAsync(new User()
        {
            UserId = Guid.NewGuid(),
            Email = email,
            PasswordHash = hashedPassword,
            UserName = userName,
        }, token);
        
        var user = await _repository.GetByEmail(email, token);
        
        return _jwtProvider.GenerateToken(user);
    }

    public async Task<string> Login(string email, string password, CancellationToken token)
    {
        var user = await _repository.GetByEmail(email, token);
        var isPassCorrect = _passwordHasher.Verify(password, user.PasswordHash);

        if (user == null) throw new Exception("Пользователь не найден");
        if (!isPassCorrect) throw new Exception("Неверный пароль");

        return _jwtProvider.GenerateToken(user);
    }
}