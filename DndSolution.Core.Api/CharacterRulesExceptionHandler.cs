using DndSolution.Application.Models.Models.Avatars;
using DndSolution.Application.Models.Models.Rules;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Core.Api;

/// <summary>
/// Превращает нарушение правил создания персонажа и ошибки картинки
/// в 400 ValidationProblemDetails
/// </summary>
public class CharacterRulesExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetails;

    public CharacterRulesExceptionHandler(IProblemDetailsService problemDetails)
    {
        _problemDetails = problemDetails;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken token)
    {
        var (errors, title) = exception switch
        {
            CharacterRulesException rules => (rules.Errors, "Персонаж не соответствует правилам"),
            AvatarException avatar => (avatar.Errors, "Картинка не подходит"),
            _ => (null, null),
        };
        if (errors is null)
        {
            return false;
        }

        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        return await _problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            Exception = exception,
            ProblemDetails = new ValidationProblemDetails(errors.ToDictionary(x => x.Key, x => x.Value))
            {
                Title = title,
                Status = StatusCodes.Status400BadRequest,
            },
        });
    }
}
