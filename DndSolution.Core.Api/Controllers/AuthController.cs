using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Security.Claims;
using Core.Api.Mappings;
using Core.Sdk;
using Core.Sdk.Dtos;
using Core.Sdk.Dtos.Character;
using Core.Sdk.Dtos.Characters;
using DndSolution.Application.Abstractions;
using DndSolution.Application.Models;
using DndSolution.Application.Models.Models.Character;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Core.Api.Controllers;

public class AuthController : ControllerBase
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private static readonly EmailAddressAttribute _emailAddressAttribute = new();

    public AuthController(SignInManager<User> signInManager, UserManager<User> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpPost("/refresh")]
    public async Task<Results<Ok<AccessTokenResponse>, UnauthorizedHttpResult, SignInHttpResult, ChallengeHttpResult>>
        Refresh([FromBody] RefreshRequest refreshRequest, [FromServices] IServiceProvider sp)
    {
        var signInManager = sp.GetRequiredService<SignInManager<User>>();
        var timeProvider = sp.GetRequiredService<TimeProvider>();
        var bearerTokenOptions = sp.GetRequiredService<IOptionsMonitor<BearerTokenOptions>>();
        var refreshTokenProtector = bearerTokenOptions.Get(IdentityConstants.BearerScheme).RefreshTokenProtector;
        var refreshTicket = refreshTokenProtector.Unprotect(refreshRequest.RefreshToken);

        // Reject the /refresh attempt with a 401 if the token expired or the security stamp validation fails
        if (refreshTicket?.Properties?.ExpiresUtc is not { } expiresUtc ||
            timeProvider.GetUtcNow() >= expiresUtc ||
            await signInManager.ValidateSecurityStampAsync(refreshTicket.Principal) is not User user)

        {
            return TypedResults.Challenge();
        }

        var newPrincipal = await signInManager.CreateUserPrincipalAsync(user);
        return TypedResults.SignIn(newPrincipal, authenticationScheme: IdentityConstants.BearerScheme);
    }
    
    [HttpPost("/login")]
    public async Task<Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult>> Login([FromBody]CustomLoginRequest request)
    {
        _signInManager.AuthenticationScheme = IdentityConstants.BearerScheme;
        var result = await _signInManager.PasswordSignInAsync(request.Login, request.Password, false, false);

        if (result.Succeeded)
        {
            return TypedResults.Empty;
        }

        return TypedResults.Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized);
    }

    [HttpPost("/register")]
    public async Task<Results<Ok, ValidationProblem>> Register([FromBody] CustomRegisterRequest request,[FromServices] IServiceProvider sp)
    {
        var userManager = sp.GetRequiredService<UserManager<User>>();

        if (!userManager.SupportsUserEmail)
        {
            throw new NotSupportedException($"{nameof(AuthController)} requires a user store with email support.");
        }

        var userStore = sp.GetRequiredService<IUserStore<User>>();
        var emailStore = (IUserEmailStore<User>)userStore;
        var email = request.Email;
        var userName = request.Login;

        if (string.IsNullOrEmpty(email) || !_emailAddressAttribute.IsValid(email))
        {
            return CreateValidationProblem(IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(email)));
        }

        var user = new User();
        await userStore.SetUserNameAsync(user, userName, CancellationToken.None);
        await emailStore.SetEmailAsync(user, email, CancellationToken.None);
        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return CreateValidationProblem(result);
        }
        
        return TypedResults.Ok();
    }
    
    private static ValidationProblem CreateValidationProblem(IdentityResult result)
    {
        // We expect a single error code and description in the normal case.
        // This could be golfed with GroupBy and ToDictionary, but perf! :P
        Debug.Assert(!result.Succeeded);
        var errorDictionary = new Dictionary<string, string[]>(1);

        foreach (var error in result.Errors)
        {
            string[] newDescriptions;

            if (errorDictionary.TryGetValue(error.Code, out var descriptions))
            {
                newDescriptions = new string[descriptions.Length + 1];
                Array.Copy(descriptions, newDescriptions, descriptions.Length);
                newDescriptions[descriptions.Length] = error.Description;
            }
            else
            {
                newDescriptions = [error.Description];
            }

            errorDictionary[error.Code] = newDescriptions;
        }

        return TypedResults.ValidationProblem(errorDictionary);
    }
}