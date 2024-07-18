using Data.Abstractions;
using Data.Repositories;
using DndSolution.Application.Abstractions;
using DndSolution.Application.Models;
using DndSolution.Neccessary;

namespace DndSolution.Application.Services;

public class AuthService : IAuthService
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRepository _userRepository;
    private readonly ITokensRepository _tokensRepository;
    private readonly ITokenHasher _tokenHasher;
    private readonly IJwtProvider _jwtProvider;
    

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtProvider jwtProvider, ITokenHasher tokenHasher, ITokensRepository tokensRepository)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
        _tokenHasher = tokenHasher;
        _tokensRepository = tokensRepository;
    }

    public async Task<TokensModel> Register(string userName, string email, string password, CancellationToken token)
    {
        var hashedPassword = _passwordHasher.Generate(password);
        var refreshToken = RandomString(12);
        
        await _userRepository.SaveUserAsync(new User()
        {
            UserId = Guid.NewGuid(),
            Email = email,
            PasswordHash = hashedPassword,
            UserName = userName,
            RefreshTokens = [new RefreshToken()
            {
                TokenHash = _tokenHasher.Generate(refreshToken),
            }]
        }, token);
        
        var user = await _userRepository.GetByEmail(email, token);

        return new TokensModel()
        {
            AccessToken = _jwtProvider.GenerateToken(user),
            RefreshToken = refreshToken,
        };
    }

    public async Task<TokensModel> Login(string email, string password, CancellationToken token)
    {
        var user = await _userRepository.GetByEmail(email, token);
        var isPassCorrect = _passwordHasher.Verify(password, user.PasswordHash);
        
        if (user == null) throw new Exception("Пользователь не найден");
        if (!isPassCorrect) throw new Exception("Неверный пароль");
        
        var refreshToken = RandomString(12);
        
        await _tokensRepository.Refresh(new RefreshToken()
        {
            TokenHash = _tokenHasher.Generate(refreshToken),
            UserId = user.Id,
        }, token);

        return new TokensModel()
        {
            AccessToken = _jwtProvider.GenerateToken(user),
            RefreshToken = refreshToken,
        };
    }

    public async Task<TokensModel> Refresh(string refreshToken, string email, CancellationToken token)
    {
        var user = await _userRepository.GetByEmail(email, token);
        var isTokenCorrect = _tokenHasher.Verify(refreshToken, user);

        if (!isTokenCorrect) throw new Exception("Неверный токен");
        
        var newToken = RandomString(12);
        
        await _tokensRepository.Refresh(new RefreshToken()
        {
            TokenHash = _tokenHasher.Generate(refreshToken),
            UserId = user.Id,
        }, token);

        return new TokensModel()
        {
            AccessToken = _jwtProvider.GenerateToken(user),
            RefreshToken = newToken,
        };
    }
    
    private static Random random = new Random();

    private static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}