using Core.Sdk.Dtos;
using Core.Sdk.Dtos.Characters;
using Core.Sdk.Enums;
using DndSolution.Application.Models.Enums;
using DndSolution.Application.Models.Models.Rules;

namespace Core.Api.Mappings;

public static class CharacterMappings
{
    public static CharacterDraft ToDraft(this CharacterDraftDto dto) => new(
        (CharacterRaceType)dto.CharacterRace,
        (CharacterClassType)dto.CharacterClass,
        (GenerationMethod)dto.GenerationMethod,
        dto.BaseStats.ToScores(),
        dto.BonusChoices.ToModel())
    {
        Background = (BackgroundType?)dto.Background,
        BackgroundSkills = dto.BackgroundSkills.ToModel(),
        RaceSkills = dto.RaceSkills.ToModel(),
        ClassSkills = dto.ClassSkills.ToModel(),
        Expertise = dto.Expertise.ToModel(),
    };

    public static CharacterDto ToDto(this CharacterWithSheet model)
    {
        var (character, sheet) = model;

        return new CharacterDto
        {
            Id = character.Id,
            Name = character.Name,
            Level = character.Level,
            CharacterClass = (CharacterClassTypeDto)character.CharacterClass,
            CharacterRace = (CharacterRaceTypeDto)character.CharacterRace,
            GenerationMethod = (GenerationMethodDto)character.GenerationMethod,
            BaseStats = AbilityScores.From(character.Stats).ToDto(),
            BonusChoices = character.BonusChoices.ToDto(),
            Background = (BackgroundTypeDto)character.Background,
            BackgroundSkills = character.BackgroundSkills.ToDto(),
            RaceSkills = character.RaceSkills.ToDto(),
            ClassSkills = character.ClassSkills.ToDto(),
            Expertise = character.Expertise.ToDto(),
            Sheet = sheet.ToDto(),
            Avatar = character.Avatar?.ToDto(character.Id),
        };
    }

    public static CharacterSheetDto ToDto(this CharacterSheet sheet) => new()
    {
        Stats = sheet.Scores.ToDto(),
        Modifiers = sheet.Modifiers.ToDto(),
        HitPoints = sheet.HitPoints,
        ArmorClass = sheet.ArmorClass,
        Initiative = sheet.Initiative,
        Speed = sheet.Speed,
        ProficiencyBonus = sheet.ProficiencyBonus,
        SavingThrows = sheet.SavingThrows
            .Select(s => new SavingThrowDto { Ability = (AbilityDto)s.Ability, Bonus = s.Bonus, Proficient = s.Proficient })
            .ToList(),
        Skills = sheet.Skills
            .Select(s => new SkillCheckDto
            {
                Skill = (SkillDto)s.Skill,
                Ability = (AbilityDto)s.Ability,
                Bonus = s.Bonus,
                Proficient = s.Proficient,
                Expertise = s.Expertise,
            })
            .ToList(),
        PassivePerception = sheet.PassivePerception,
    };

    private static AbilityScores ToScores(this CharacterStatsDto dto) => new(
        dto.Strength, dto.Dexterity, dto.Constitution, dto.Intelligence, dto.Wisdom, dto.Charisma);

    private static CharacterStatsDto ToDto(this AbilityScores scores) => new()
    {
        Strength = scores.Strength,
        Dexterity = scores.Dexterity,
        Constitution = scores.Constitution,
        Intelligence = scores.Intelligence,
        Wisdom = scores.Wisdom,
        Charisma = scores.Charisma,
    };
}
