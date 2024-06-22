using Data.Entities.Entities;
using Data.Entities.Enums;
using DndSolution.Application.Models.Models;
using DndSolution.Neccessary;

namespace DndSolution.Application.Services.Mappers;

public static class CharacterMapper
{
    public static CharacterEntity MapToEntity(Character model)
    {
        return new CharacterEntity
        {
            Name = model.Name,
            CharacterClass = (EntityCharacterClass)model.CharacterClass,
            CharacterRace = (EntityCharacterRace)model.CharacterRace,
            Level = model.Level,
            Xp = 0,
            CharacterId = GetNewId(),
        };
    }

    private static string GetNewId()
    {
        var chars = DndConstants.Alphanumeric;
        var characterId = new char[16];
        var rand = new Random();
        
        for (int i = 0; i < characterId.Length; i++)
        {
            characterId[i] = chars[rand.Next(chars.Length)];
        }

        return new String(characterId);
    }
}