using Core.Api.Mappings;
using Core.Sdk.Dtos;
using DndSolution.Application.Abstractions;
using DndSolution.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Core.Api.Controllers;

/// <summary>
/// Контроллер работы с персонажами
/// </summary>
[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IAuthService _service;

    public AuthController(ILogger<AuthController> logger, IAuthService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken token)
    {
        try
        {
            var jwt = await _service.Register(request.UserName, request.Email, request.Password, token);

            return Ok(jwt);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken token)
    {
        try
        {
            var jwt = await _service.Login(request.Email, request.Password, token);

            return Ok(jwt);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] string refreshToken, string email, CancellationToken token)
    {
        try
        {
            var jwt = await _service.Refresh(refreshToken, email, token);

            return Ok(jwt);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

}