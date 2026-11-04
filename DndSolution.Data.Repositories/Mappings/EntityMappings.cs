using Data.Entities;
using DndSolution.Application.Models.Models;
using DndSolution.Application.Models.Models.Avatars;
using DndSolution.Application.Models.Models.Backgrounds;
using DndSolution.Application.Models.Models.Character;
using DndSolution.Application.Models.Models.Classes;
using DndSolution.Application.Models.Models.Races;

namespace Data.Repositories.Mappings;

/// <summary>
/// Перевод между доменными моделями и сущностями хранения.
/// Слой приложения работает только с моделями, о сущностях знают лишь репозитории.
/// </summary>
internal static class EntityMappings
{
    public static Character ToModel(this CharacterEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Level = entity.Level,
        CharacterClass = entity.CharacterClass,
        CharacterRace = entity.CharacterRace,
        GenerationMethod = entity.GenerationMethod,
        BonusChoices = entity.BonusChoices.ToList(),
        Background = entity.Background,
        BackgroundSkills = entity.BackgroundSkills.ToList(),
        RaceSkills = entity.RaceSkills.ToList(),
        ClassSkills = entity.ClassSkills.ToList(),
        Expertise = entity.Expertise.ToList(),
        Stats = new CharacterStats
        {
            Strength = entity.Stats.Strength,
            Dexterity = entity.Stats.Dexterity,
            Constitution = entity.Stats.Constitution,
            Intelligence = entity.Stats.Intelligence,
            Wisdom = entity.Stats.Wisdom,
            Charisma = entity.Stats.Charisma,
        },
        Avatar = entity.Avatar?.ToModel(),
    };

    public static CharacterAvatar ToModel(this CharacterAvatarEntity entity) => new()
    {
        FileName = entity.FileName,
        HasTransparency = entity.HasTransparency,
        Mode = entity.Mode,
        OffsetX = entity.OffsetX,
        OffsetY = entity.OffsetY,
        Scale = entity.Scale,
    };

    public static CharacterAvatarEntity ToEntity(this CharacterAvatar model) => new()
    {
        FileName = model.FileName,
        HasTransparency = model.HasTransparency,
        Mode = model.Mode,
        OffsetX = model.OffsetX,
        OffsetY = model.OffsetY,
        Scale = model.Scale,
    };

    public static CharacterEntity ToEntity(this Character model, Guid userId, DateTime creationTime) => new()
    {
        Id = model.Id,
        UserId = userId,
        CreationTime = creationTime,
        Name = model.Name,
        Level = model.Level,
        CharacterClass = model.CharacterClass,
        CharacterRace = model.CharacterRace,
        GenerationMethod = model.GenerationMethod,
        BonusChoices = model.BonusChoices.ToList(),
        Background = model.Background,
        BackgroundSkills = model.BackgroundSkills.ToList(),
        RaceSkills = model.RaceSkills.ToList(),
        ClassSkills = model.ClassSkills.ToList(),
        Expertise = model.Expertise.ToList(),
        Stats = new CharacterStatsEntity
        {
            Strength = model.Stats.Strength,
            Dexterity = model.Stats.Dexterity,
            Constitution = model.Stats.Constitution,
            Intelligence = model.Stats.Intelligence,
            Wisdom = model.Stats.Wisdom,
            Charisma = model.Stats.Charisma,
        },
        Avatar = model.Avatar?.ToEntity(),
    };

    public static Race ToModel(this RaceEntity entity) => new()
    {
        Type = entity.Type,
        Name = entity.Name,
        Description = entity.Description,
        Age = entity.Age,
        Size = entity.Size,
        Language = entity.Language,
        Vision = entity.Vision,
        Traits = entity.Traits,
        Skills = entity.Skills.ToList(),
        SkillChoices = entity.SkillChoices,
        Asi = entity.Asi
            .Select(x => new Asi { Stat = x.Stat, Value = x.Value, Ability = x.Ability, Choices = x.Choices })
            .ToList(),
        Speed = entity.Speed.Select(x => new Speed { Type = x.Type, Value = x.Value }).ToList(),
    };

    public static RaceEntity ToEntity(this Race model) => new()
    {
        Type = model.Type,
        Name = model.Name,
        Description = model.Description,
        Age = model.Age,
        Size = model.Size,
        Language = model.Language,
        Vision = model.Vision,
        Traits = model.Traits,
        Skills = model.Skills.ToList(),
        SkillChoices = model.SkillChoices,
        Asi = model.Asi
            .Select(x => new AsiEntity { Stat = x.Stat, Value = x.Value, Ability = x.Ability, Choices = x.Choices })
            .ToList(),
        Speed = model.Speed.Select(x => new SpeedEntity { Type = x.Type, Value = x.Value }).ToList(),
    };

    public static CharacterClass ToModel(this CharacterClassEntity entity) => new()
    {
        Type = entity.Type,
        Name = entity.Name,
        Description = entity.Description,
        HitDie = entity.HitDie,
        SavingThrows = entity.SavingThrows.ToList(),
        SkillChoices = entity.SkillChoices,
        SkillOptions = entity.SkillOptions.ToList(),
        ExpertiseChoices = entity.ExpertiseChoices,
        HitDice = entity.HitDice,
        HpAt1stLevel = entity.HpAt1stLevel,
        HpAtHigherLevels = entity.HpAtHigherLevels,
        ProfArmor = entity.ProfArmor,
        ProfWeapon = entity.ProfWeapon,
        ProfTool = entity.ProfTool,
        ProfSavingThrows = entity.ProfSavingThrows,
        ProfSkills = entity.ProfSkills,
        Equipment = entity.Equipment,
        SpellCastingAbility = entity.SpellCastingAbility,
    };

    public static CharacterClassEntity ToEntity(this CharacterClass model) => new CharacterClassEntity().Apply(model);

    /// <summary>
    /// Переносит данные класса в существующую сущность (для обновления справочника)
    /// </summary>
    public static CharacterClassEntity Apply(this CharacterClassEntity entity, CharacterClass model)
    {
        entity.Type = model.Type;
        entity.Name = model.Name;
        entity.Description = model.Description;
        entity.HitDie = model.HitDie;
        entity.SavingThrows = model.SavingThrows.ToList();
        entity.SkillChoices = model.SkillChoices;
        entity.SkillOptions = model.SkillOptions.ToList();
        entity.ExpertiseChoices = model.ExpertiseChoices;
        entity.HitDice = model.HitDice;
        entity.HpAt1stLevel = model.HpAt1stLevel;
        entity.HpAtHigherLevels = model.HpAtHigherLevels;
        entity.ProfArmor = model.ProfArmor;
        entity.ProfWeapon = model.ProfWeapon;
        entity.ProfTool = model.ProfTool;
        entity.ProfSavingThrows = model.ProfSavingThrows;
        entity.ProfSkills = model.ProfSkills;
        entity.Equipment = model.Equipment;
        entity.SpellCastingAbility = model.SpellCastingAbility;
        return entity;
    }

    public static Background ToModel(this BackgroundEntity entity) => new()
    {
        Type = entity.Type,
        Name = entity.Name,
        Description = entity.Description,
        Skills = entity.Skills.ToList(),
        SkillChoices = entity.SkillChoices,
        Tools = entity.Tools,
        Languages = entity.Languages,
        Equipment = entity.Equipment,
        Feature = entity.Feature,
    };

    public static BackgroundEntity ToEntity(this Background model) => new BackgroundEntity().Apply(model);

    /// <summary>
    /// Переносит данные предыстории в существующую сущность (для обновления справочника)
    /// </summary>
    public static BackgroundEntity Apply(this BackgroundEntity entity, Background model)
    {
        entity.Type = model.Type;
        entity.Name = model.Name;
        entity.Description = model.Description;
        entity.Skills = model.Skills.ToList();
        entity.SkillChoices = model.SkillChoices;
        entity.Tools = model.Tools;
        entity.Languages = model.Languages;
        entity.Equipment = model.Equipment;
        entity.Feature = model.Feature;
        return entity;
    }
}
