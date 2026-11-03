using Core.Sdk.Enums;

namespace Core.Sdk.Dtos;

/// <summary>
/// Предыстория
/// </summary>
public class BackgroundDto
{
    public BackgroundTypeDto Type { get; init; }

    public required string Name { get; init; }

    public required string Description { get; init; }

    /// <summary>
    /// Навыки, которые даёт предыстория
    /// </summary>
    public required List<SkillDto> Skills { get; init; }

    /// <summary>
    /// Сколько навыков игрок выбирает сам (своя предыстория)
    /// </summary>
    public int SkillChoices { get; init; }

    public required string Tools { get; init; }

    public required string Languages { get; init; }

    public required string Equipment { get; init; }

    /// <summary>
    /// Умение предыстории
    /// </summary>
    public required string Feature { get; init; }
}
