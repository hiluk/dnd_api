using DndSolution.Application.Models.Enums;

namespace Data.Entities;

/// <summary>
/// Предыстория (таблица backgrounds)
/// </summary>
public class BackgroundEntity
{
    public int Id { get; set; }

    public BackgroundType Type { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<Skill> Skills { get; set; } = [];

    public int SkillChoices { get; set; }

    public string Tools { get; set; } = string.Empty;

    public string Languages { get; set; } = string.Empty;

    public string Equipment { get; set; } = string.Empty;

    public string Feature { get; set; } = string.Empty;
}
