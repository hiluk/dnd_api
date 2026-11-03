using Core.Sdk.Dtos.Characters;
using DndSolution.Application.Models.Models.Rules;

namespace Core.Api.Mappings;

public static class AbilityRollMappings
{
    public static AbilityRollDto ToDto(this AbilityRoll model) => new()
    {
        Values = model.Values.ToList(),
        Token = model.Token,
    };
}
