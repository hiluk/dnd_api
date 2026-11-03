using DndSolution.Application.Models.Models.Rules;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Core.Api;

/// <summary>
/// Превращает нарушение правил создания персонажа в 400 ValidationProblemDetails
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
        if (exception is not CharacterRulesException rules)
        {
            return false;
        }

        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        return await _problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            Exception = exception,
            ProblemDetails = new ValidationProblemDetails(rules.Errors.ToDictionary(x => x.Key, x => x.Value))
            {
                Title = "Персонаж не соответствует правилам",
                Status = StatusCodes.Status400BadRequest,
            },
        });
    }
}
