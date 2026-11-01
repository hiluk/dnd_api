using DndSolution.Application.Models.Enums;

namespace DndSolution.Application.Models.Models.Backgrounds;

/// <summary>
/// Предыстория персонажа
/// </summary>
public class Background
{
    public int Id { get; set; }

    public BackgroundType Type { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Навыки, которые даёт предыстория
    /// </summary>
    public List<Skill> Skills { get; set; } = [];

    /// <summary>
    /// Сколько навыков игрок выбирает сам (у своей предыстории)
    /// </summary>
    public int SkillChoices { get; set; }

    public string Tools { get; set; } = string.Empty;

    public string Languages { get; set; } = string.Empty;

    public string Equipment { get; set; } = string.Empty;

    /// <summary>
    /// Умение предыстории
    /// </summary>
    public string Feature { get; set; } = string.Empty;
}
