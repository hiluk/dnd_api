using Data.Abstractions;

namespace Data.Repositories;

/// <summary>
/// Картинки персонажей в папке на диске
/// </summary>
public sealed class FileAvatarStorage : IAvatarStorage
{
    private readonly string _root;

    public FileAvatarStorage(string root)
    {
        _root = Path.GetFullPath(root);
        Directory.CreateDirectory(_root);
    }

    public async Task SaveAsync(string fileName, byte[] content, CancellationToken token)
    {
        var path = PathOf(fileName);
        // Пишем во временный файл и переименовываем: читатель не увидит половину файла
        var temp = path + ".tmp";
        await File.WriteAllBytesAsync(temp, content, token);
        File.Move(temp, path, overwrite: true);
    }

    public Stream? Open(string fileName)
    {
        var path = PathOf(fileName);
        return File.Exists(path) ? File.OpenRead(path) : null;
    }

    public void Delete(string fileName)
    {
        var path = PathOf(fileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    /// <summary>
    /// Имя файла генерирует сервер, но на всякий случай не даём выйти из папки
    /// </summary>
    private string PathOf(string fileName)
    {
        if (fileName != Path.GetFileName(fileName) || fileName.Length == 0)
        {
            throw new ArgumentException("Недопустимое имя файла", nameof(fileName));
        }

        return Path.Combine(_root, fileName);
    }
}
