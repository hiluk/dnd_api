using System.ComponentModel.DataAnnotations;

namespace Core.Sdk.Dtos.Characters;

/// <summary>
/// Запрос на создание персонажа
/// </summary>
public class CreateCharacterRequest : CharacterDraftDto
{
    /// <summary>
    /// Имя персонажа
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [StringLength(64)]
    public string Name { get; init; } = string.Empty;
}
