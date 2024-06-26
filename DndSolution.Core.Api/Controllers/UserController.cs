using Core.Api.Mappings;
using Core.Sdk.Dtos;
using DndSolution.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Core.Api.Controllers;

/// <summary>
/// Контроллер работы с персонажами
/// </summary>
[ApiController]
[Route("api/v1/user")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _service;

    public UserController(ILogger<UserController> logger, IUserService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpPost("saveUser")]
    public async Task SaveUser(UserDto dto, CancellationToken token)
    {
        try
        {
            var model = UserMapper.MapToModel(dto);
            await _service.SaveUserAsync(model, token);
        }
        catch (Exception e)
        {
            _logger.LogError(e,"Не удалось сохранить пользователя");
            throw;
        }
    }
    
    
}