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
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _service;

    public UserController(ILogger<UserController> logger, IUserService service)
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

}