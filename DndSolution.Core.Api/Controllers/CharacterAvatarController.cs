using System.Security.Claims;
using Core.Api.Mappings;
using Core.Sdk.Dtos.Characters;
using DndSolution.Application.Abstractions;
using DndSolution.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Core.Api.Controllers;

/// <summary>
/// Своя картинка персонажа
/// </summary>
[ApiController]
[Authorize]
[Route("/characters/{id:guid}/avatar")]
public class CharacterAvatarController : ControllerBase
{
    /// <summary>
    /// Запас на заголовки multipart сверх размера самого файла
    /// </summary>
    private const long RequestLimit = SkiaImageProcessor.MaxFileBytes + 64 * 1024;

    private readonly ICharacterAvatarService _service;

    /// <summary>
    /// .ctor
    /// </summary>
    public CharacterAvatarController(ICharacterAvatarService service)
    {
        _service = service;
    }

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? throw new InvalidOperationException("В токене нет Id пользователя"));

    /// <summary>
    /// Загрузить картинку: JPEG, PNG или WebP до 5 МБ. Заменяет прежнюю,
    /// вид сбрасывается на «в рамке».
    /// </summary>
    /// <param name="id">Id персонажа</param>
    /// <param name="file">Файл картинки</param>
    /// <param name="token">Токен отмены операции</param>
    [HttpPut]
    [RequestSizeLimit(RequestLimit)]
    [RequestFormLimits(MultipartBodyLengthLimit = RequestLimit)]
    [ProducesResponseType<CharacterAvatarDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Upload([FromRoute] Guid id, IFormFile? file, CancellationToken token)
    {
        if (file is null)
        {
            return ValidationProblem(new ValidationProblemDetails(
                new Dictionary<string, string[]> { ["File"] = ["Прикрепите файл картинки"] }));
        }

        await using var content = file.OpenReadStream();
        var avatar = await _service.UploadAsync(CurrentUserId, id, content, token);

        return avatar is null ? NotFoundProblem() : Ok(avatar.ToDto(id));
    }

    /// <summary>
    /// Изменить вид («в рамке» или «фигура») и кадр
    /// </summary>
    /// <param name="id">Id персонажа</param>
    /// <param name="request">Вид, сдвиг и увеличение</param>
    /// <param name="token">Токен отмены операции</param>
    [HttpPatch]
    [ProducesResponseType<CharacterAvatarDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id, [FromBody] UpdateAvatarRequest request, CancellationToken token)
    {
        var avatar = await _service.UpdateLayoutAsync(CurrentUserId, id, request.ToLayout(), token);

        return avatar is null ? NotFoundProblem() : Ok(avatar.ToDto(id));
    }

    /// <summary>
    /// Файл картинки. Отдаётся только владельцу персонажа
    /// </summary>
    /// <param name="id">Id персонажа</param>
    /// <param name="token">Токен отмены операции</param>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken token)
    {
        var file = await _service.GetFileAsync(CurrentUserId, id, token);
        if (file is null)
        {
            return NotFoundProblem();
        }

        // Адрес содержит версию файла, поэтому кэшировать можно надолго, но только у себя
        Response.Headers[HeaderNames.CacheControl] = "private, max-age=31536000, immutable";
        return File(file.Content, file.ContentType);
    }

    /// <summary>
    /// Вернуть стандартную картинку класса
    /// </summary>
    /// <param name="id">Id персонажа</param>
    /// <param name="token">Токен отмены операции</param>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
    {
        var deleted = await _service.DeleteAsync(CurrentUserId, id, token);

        return deleted ? NoContent() : NotFoundProblem();
    }

    private ObjectResult NotFoundProblem() =>
        Problem("Персонаж или его картинка не найдены", statusCode: StatusCodes.Status404NotFound);
}
