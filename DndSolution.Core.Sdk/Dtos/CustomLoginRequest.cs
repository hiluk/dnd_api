using System.ComponentModel.DataAnnotations;

namespace Core.Sdk;

/// <summary>
/// Запрос на вход
/// </summary>
public sealed class CustomLoginRequest
{
    /// <summary>
    /// Логин пользователя
    /// </summary>
    [Required]
    public string Login { get; init; } = string.Empty;

    /// <summary>
    /// Пароль пользователя
    /// </summary>
    [Required]
    public string Password { get; init; } = string.Empty;
}
