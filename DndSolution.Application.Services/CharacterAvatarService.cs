using Data.Abstractions;
using DndSolution.Application.Abstractions;
using DndSolution.Application.Models.Enums;
using DndSolution.Application.Models.Models.Avatars;

namespace DndSolution.Application.Services;

public sealed class CharacterAvatarService : ICharacterAvatarService
{
    public const double MinScale = 1;
    public const double MaxScale = 4;

    private readonly ICharactersRepository _characters;
    private readonly IAvatarStorage _storage;
    private readonly IImageProcessor _processor;

    public CharacterAvatarService(
        ICharactersRepository characters, IAvatarStorage storage, IImageProcessor processor)
    {
        _characters = characters;
        _storage = storage;
        _processor = processor;
    }

    public async Task<CharacterAvatar?> UploadAsync(
        Guid userId, Guid characterId, Stream content, CancellationToken token)
    {
        var character = await _characters.GetCharacterByIdAsync(userId, characterId, token);
        if (character is null)
        {
            return null;
        }

        var bytes = await ReadLimitedAsync(content, token);
        var image = _processor.Process(bytes);

        var avatar = new CharacterAvatar
        {
            FileName = $"{Guid.NewGuid():N}.{image.Extension}",
            HasTransparency = image.HasTransparency,
        };
        await _storage.SaveAsync(avatar.FileName, image.Content, token);

        if (!await _characters.SetAvatarAsync(userId, characterId, avatar, token))
        {
            // Персонажа удалили, пока шла загрузка
            _storage.Delete(avatar.FileName);
            return null;
        }

        if (character.Avatar is not null)
        {
            _storage.Delete(character.Avatar.FileName);
        }

        return avatar;
    }

    public async Task<CharacterAvatar?> UpdateLayoutAsync(
        Guid userId, Guid characterId, AvatarLayout layout, CancellationToken token)
    {
        var character = await _characters.GetCharacterByIdAsync(userId, characterId, token);
        if (character?.Avatar is null)
        {
            return null;
        }

        var errors = new Dictionary<string, string[]>();
        if (!Enum.IsDefined(layout.Mode))
        {
            errors["Mode"] = ["Неизвестный вид картинки"];
        }
        else if (layout.Mode == AvatarMode.Figure && !character.Avatar.HasTransparency)
        {
            errors["Mode"] = ["Фигурой можно показать только картинку с прозрачным фоном"];
        }

        if (!double.IsFinite(layout.OffsetX) || layout.OffsetX is < -1 or > 1)
        {
            errors["OffsetX"] = ["Сдвиг должен быть от -1 до 1"];
        }

        if (!double.IsFinite(layout.OffsetY) || layout.OffsetY is < -1 or > 1)
        {
            errors["OffsetY"] = ["Сдвиг должен быть от -1 до 1"];
        }

        if (!double.IsFinite(layout.Scale) || layout.Scale is < MinScale or > MaxScale)
        {
            errors["Scale"] = [$"Увеличение должно быть от {MinScale} до {MaxScale}"];
        }

        if (errors.Count > 0)
        {
            throw new AvatarException(errors);
        }

        var updated = character.Avatar with
        {
            Mode = layout.Mode,
            OffsetX = layout.OffsetX,
            OffsetY = layout.OffsetY,
            Scale = layout.Scale,
        };

        return await _characters.SetAvatarAsync(userId, characterId, updated, token) ? updated : null;
    }

    public async Task<AvatarFile?> GetFileAsync(Guid userId, Guid characterId, CancellationToken token)
    {
        var character = await _characters.GetCharacterByIdAsync(userId, characterId, token);
        if (character?.Avatar is null)
        {
            return null;
        }

        var stream = _storage.Open(character.Avatar.FileName);
        if (stream is null)
        {
            return null;
        }

        var contentType = character.Avatar.FileName.EndsWith(".png") ? "image/png" : "image/jpeg";
        return new AvatarFile(stream, contentType);
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid characterId, CancellationToken token)
    {
        var character = await _characters.GetCharacterByIdAsync(userId, characterId, token);
        if (character is null)
        {
            return false;
        }

        if (character.Avatar is not null)
        {
            await _characters.SetAvatarAsync(userId, characterId, null, token);
            _storage.Delete(character.Avatar.FileName);
        }

        return true;
    }

    /// <summary>
    /// Читаем не больше лимита, чтобы большой файл не занял память целиком
    /// </summary>
    private static async Task<byte[]> ReadLimitedAsync(Stream content, CancellationToken token)
    {
        using var buffer = new MemoryStream();
        var chunk = new byte[81920];
        int read;
        while ((read = await content.ReadAsync(chunk, token)) > 0)
        {
            if (buffer.Length + read > SkiaImageProcessor.MaxFileBytes)
            {
                throw new AvatarException("File", "Файл больше 5 МБ");
            }

            buffer.Write(chunk, 0, read);
        }

        return buffer.ToArray();
    }
}
