using Core.Sdk.Dtos.Characters;

namespace Core.Sdk.Dtos.Character;

public class CharacterFullDto
{
    public string email { get; set; }
    
    public CharacterDto Character { get; set; }
}