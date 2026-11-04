using System.Security.Claims;
using Core.Api.Mappings;
using Core.Sdk.Dtos.Session;
using DndSolution.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Api.Controllers;

/// <summary>
/// Игровая сессия персонажа: хиты, спасброски от смерти, отдых, состояния и журнал
/// </summary>
[ApiController]
[Authorize]
[Route("/characters/{id:guid}")]
public class SessionController : ControllerBase
{
    private readonly ISessionService _service;

    /// <summary>
    /// .ctor
    /// </summary>
    public SessionController(ISessionService service)
    {
        _service = service;
    }

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? throw new InvalidOperationException("В токене нет Id пользователя"));

    /// <summary>
    /// Текущее состояние сессии. Если сессия не начиналась — полные хиты
    /// </summary>
    /// <param name="id">Id персонажа</param>
    /// <param name="token">Токен отмены операции</param>
    [HttpGet("session")]
    [ProducesResponseType<SessionDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken token)
    {
        var view = await _service.GetAsync(CurrentUserId, id, token);

        return view is null ? NotFoundProblem() : Ok(view.ToDto());
    }

    /// <summary>
    /// Применить действие. Повтор с тем же Id не применяется — возвращается уже записанный итог,
    /// поэтому очередь клиента можно безопасно отправлять повторно.
    /// Действие, недоступное по правилам, — 400
    /// </summary>
    /// <param name="id">Id персонажа</param>
    /// <param name="request">Действие</param>
    /// <param name="token">Токен отмены операции</param>
    [HttpPost("actions")]
    [ProducesResponseType<SessionActionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Apply(
        [FromRoute] Guid id, [FromBody] SessionActionRequest request, CancellationToken token)
    {
        if (request.Id == Guid.Empty)
        {
            return ValidationProblem(new ValidationProblemDetails(
                new Dictionary<string, string[]> { ["Id"] = ["Укажите Id действия"] }));
        }

        var applied = await _service.ApplyAsync(
            CurrentUserId, id, request.Id, request.ToAction(), request.CreatedAt, token);
        if (applied is not { } result)
        {
            return NotFoundProblem();
        }

        return Ok(new SessionActionResponse
        {
            Session = result.View.ToDto(),
            Event = result.Event.ToDto(),
        });
    }

    /// <summary>
    /// Журнал сессии от новых к старым
    /// </summary>
    /// <param name="id">Id персонажа</param>
    /// <param name="before">Вернуть записи раньше этого времени (для подгрузки)</param>
    /// <param name="limit">Сколько записей, от 1 до 200</param>
    /// <param name="token">Токен отмены операции</param>
    [HttpGet("events")]
    [ProducesResponseType<List<SessionEventDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Events(
        [FromRoute] Guid id, [FromQuery] DateTime? before, [FromQuery] int limit = 50, CancellationToken token = default)
    {
        var events = await _service.GetEventsAsync(CurrentUserId, id, before, limit, token);

        return events is null ? NotFoundProblem() : Ok(events.Select(e => e.ToDto()).ToList());
    }

    private ObjectResult NotFoundProblem() =>
        Problem("Персонаж не найден", statusCode: StatusCodes.Status404NotFound);
}
