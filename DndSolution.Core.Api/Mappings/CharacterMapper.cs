using Core.Sdk.Dtos;
using Core.Sdk.Dtos.Characters;
using Core.Sdk.Enums;
using DndSolution.Application.Models.Enums;
using DndSolution.Application.Models.Models.Rules;

namespace Core.Api.Mappings;

public static class CharacterMapper
{
    public static CharacterDraft MapToDraft(CharacterDraftDto dto) => new(
        (CharacterRaceType)dto.CharacterRace,
        (CharacterClassType)dto.CharacterClass,
        (GenerationMethod)dto.GenerationMethod,
        MapToScores(dto.BaseStats),
        dto.BonusChoices.Select(a => (Ability)a).ToList())
    {
        Background = (BackgroundType)dto.Background,
        BackgroundSkills = Skills(dto.BackgroundSkills),
        RaceSkills = Skills(dto.RaceSkills),
        ClassSkills = Skills(dto.ClassSkills),
        Expertise = Skills(dto.Expertise),
    };

    private static List<Skill> Skills(IEnumerable<SkillDto> skills) => skills.Select(s => (Skill)s).ToList();

    private static List<SkillDto> Skills(IEnumerable<Skill> skills) => skills.Select(s => (SkillDto)s).ToList();

    public static CharacterDto MapToDto(CharacterWithSheet model)
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
            BaseStats = MapToDto(AbilityScores.From(character.Stats)),
            BonusChoices = character.BonusChoices.Select(a => (AbilityDto)a).ToList(),
            Background = (BackgroundTypeDto)character.Background,
            BackgroundSkills = Skills(character.BackgroundSkills),
            RaceSkills = Skills(character.RaceSkills),
            ClassSkills = Skills(character.ClassSkills),
            Expertise = Skills(character.Expertise),
            Sheet = MapToDto(sheet),
        };
    }

    public static CharacterSheetDto MapToDto(CharacterSheet sheet) => new()
    {
        Stats = MapToDto(sheet.Scores),
        Modifiers = MapToDto(sheet.Modifiers),
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

    private static AbilityScores MapToScores(CharacterStatsDto dto) => new(
        dto.Strength, dto.Dexterity, dto.Constitution, dto.Intelligence, dto.Wisdom, dto.Charisma);

    private static CharacterStatsDto MapToDto(AbilityScores scores) => new()
    {
        Strength = scores.Strength,
        Dexterity = scores.Dexterity,
        Constitution = scores.Constitution,
        Intelligence = scores.Intelligence,
        Wisdom = scores.Wisdom,
        Charisma = scores.Charisma,
    };
}
