using DndSolution.Application.Models.Models.Avatars;

namespace DndSolution.Application.Abstractions;

/// <summary>
/// Своя картинка персонажа
/// </summary>
public interface ICharacterAvatarService
{
    /// <summary>
    /// Загрузить картинку. Заменяет прежнюю; вид сбрасывается на «в рамке».
    /// </summary>
    /// <returns>Новая картинка; null — персонаж не найден</returns>
    /// <exception cref="AvatarException">Файл не картинка, слишком большой или повреждён</exception>
    public Task<CharacterAvatar?> UploadAsync(Guid userId, Guid characterId, Stream content, CancellationToken token);

    /// <summary>
    /// Изменить вид и кадр
    /// </summary>
    /// <returns>Обновлённая картинка; null — нет персонажа или своей картинки</returns>
    /// <exception cref="AvatarException">Неверные значения или «фигура» без прозрачного фона</exception>
    public Task<CharacterAvatar?> UpdateLayoutAsync(
        Guid userId, Guid characterId, AvatarLayout layout, CancellationToken token);

    /// <summary>
    /// Файл картинки; null — нет персонажа или своей картинки
    /// </summary>
    public Task<AvatarFile?> GetFileAsync(Guid userId, Guid characterId, CancellationToken token);

    /// <summary>
    /// Вернуть стандартную картинку и удалить файл
    /// </summary>
    /// <returns>false — персонаж не найден</returns>
    public Task<bool> DeleteAsync(Guid userId, Guid characterId, CancellationToken token);
}

/// <summary>
/// Проверка и подготовка загруженной картинки
/// </summary>
public interface IImageProcessor
{
    /// <exception cref="AvatarException">Файл не картинка нужного формата или слишком большой</exception>
    public ProcessedImage Process(byte[] content);
}
