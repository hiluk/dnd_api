using System.ComponentModel.DataAnnotations;

namespace Core.Sdk.Dtos;

/// <summary>
/// Запрос на регистрацию
/// </summary>
public class CustomRegisterRequest
{
    /// <summary>
    /// Эмейл пользователя
    /// </summary>
    [Required]
    [EmailAddress]
    public required string Email { get; init; }

    /// <summary>
    /// Логин пользователя
    /// </summary>
    [Required]
    public required string Login { get; init; }

    /// <summary>
    /// Пароль пользователя
    /// </summary>
    [Required]
    public required string Password { get; init; }
}
