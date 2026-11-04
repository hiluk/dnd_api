namespace Data.Abstractions;

/// <summary>
/// Хранилище файлов картинок персонажей
/// </summary>
public interface IAvatarStorage
{
    public Task SaveAsync(string fileName, byte[] content, CancellationToken token);

    /// <returns>null — файла нет</returns>
    public Stream? Open(string fileName);

    /// <summary>
    /// Удалить файл; отсутствующий файл — не ошибка
    /// </summary>
    public void Delete(string fileName);
}
