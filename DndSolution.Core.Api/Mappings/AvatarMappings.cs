using Core.Sdk.Dtos.Characters;
using Core.Sdk.Enums;
using DndSolution.Application.Models.Enums;
using DndSolution.Application.Models.Models.Avatars;

namespace Core.Api.Mappings;

/// <summary>
/// Перевод своей картинки персонажа между моделью и DTO
/// </summary>
public static class AvatarMappings
{
    /// <summary>
    /// Имя файла в адресе служит версией: после новой загрузки адрес другой
    /// и клиент не покажет старую картинку из кэша
    /// </summary>
    public static CharacterAvatarDto ToDto(this CharacterAvatar avatar, Guid characterId) => new()
    {
        Url = $"/characters/{characterId}/avatar?v={Path.GetFileNameWithoutExtension(avatar.FileName)}",
        HasTransparency = avatar.HasTransparency,
        Mode = (AvatarModeDto)avatar.Mode,
        OffsetX = avatar.OffsetX,
        OffsetY = avatar.OffsetY,
        Scale = avatar.Scale,
    };

    public static AvatarLayout ToLayout(this UpdateAvatarRequest request) =>
        new((AvatarMode)request.Mode, request.OffsetX, request.OffsetY, request.Scale);
}
