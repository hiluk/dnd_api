using Microsoft.AspNetCore.Identity;

namespace DndSolution.Application.Models;

/// <summary>
/// Пользователь приложения
/// </summary>
public class User : IdentityUser<Guid>
{
}
