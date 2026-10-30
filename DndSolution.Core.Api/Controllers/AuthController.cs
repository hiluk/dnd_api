using Core.Sdk;
using Core.Sdk.Dtos;
using DndSolution.Application.Models;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Core.Api.Controllers;

/// <summary>
/// Регистрация, вход и обновление токенов
/// </summary>
[ApiController]
public class AuthController : ControllerBase
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    /// <summary>
    /// .ctor
    /// </summary>
    public AuthController(SignInManager<User> signInManager, UserManager<User> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    /// <summary>
    /// Зарегистрировать нового пользователя
    /// </summary>
    [HttpPost("/register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<Results<Ok, ValidationProblem>> Register([FromBody] CustomRegisterRequest request)
    {
        var user = new User
        {
            UserName = request.Login.Trim(),
            Email = request.Email.Trim(),
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        return result.Succeeded
            ? TypedResults.Ok()
            : CreateValidationProblem(result);
    }

    /// <summary>
    /// Войти по логину или эмейлу. Возвращает access и refresh токены
    /// </summary>
    [HttpPost("/login")]
    [ProducesResponseType<AccessTokenResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<Results<EmptyHttpResult, ProblemHttpResult>> Login([FromBody] CustomLoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Login)
                   ?? await _userManager.FindByEmailAsync(request.Login);

        if (user is null)
        {
            return InvalidCredentials();
        }

        _signInManager.AuthenticationScheme = IdentityConstants.BearerScheme;
        var result = await _signInManager.PasswordSignInAsync(user, request.Password, isPersistent: false, lockoutOnFailure: true);

        if (result.IsLockedOut)
        {
            return TypedResults.Problem("Слишком много неудачных попыток, попробуйте позже",
                statusCode: StatusCodes.Status401Unauthorized);
        }

        // При успехе SignInManager сам записывает токены в ответ
        return result.Succeeded ? TypedResults.Empty : InvalidCredentials();
    }

    /// <summary>
    /// Обновить пару токенов по refresh токену
    /// </summary>
    [HttpPost("/refresh")]
    [ProducesResponseType<AccessTokenResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<Results<SignInHttpResult, ChallengeHttpResult>> Refresh(
        [FromBody] RefreshRequest refreshRequest,
        [FromServices] TimeProvider timeProvider,
        [FromServices] IOptionsMonitor<BearerTokenOptions> bearerTokenOptions)
    {
        var refreshTokenProtector = bearerTokenOptions.Get(IdentityConstants.BearerScheme).RefreshTokenProtector;
        var refreshTicket = refreshTokenProtector.Unprotect(refreshRequest.RefreshToken);

        // 401, если токен просрочен или не прошла проверка security stamp
        if (refreshTicket?.Properties?.ExpiresUtc is not { } expiresUtc ||
            timeProvider.GetUtcNow() >= expiresUtc ||
            await _signInManager.ValidateSecurityStampAsync(refreshTicket.Principal) is not { } user)
        {
            return TypedResults.Challenge();
        }

        var newPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
        return TypedResults.SignIn(newPrincipal, authenticationScheme: IdentityConstants.BearerScheme);
    }

    private static ProblemHttpResult InvalidCredentials() =>
        TypedResults.Problem("Неверный логин или пароль", statusCode: StatusCodes.Status401Unauthorized);

    private static ValidationProblem CreateValidationProblem(IdentityResult result)
    {
        var errors = result.Errors
            .GroupBy(e => e.Code)
            .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

        return TypedResults.ValidationProblem(errors);
    }
}
