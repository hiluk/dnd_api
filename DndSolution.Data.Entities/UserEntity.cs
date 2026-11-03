using Microsoft.AspNetCore.Identity;

namespace Data.Entities;

/// <summary>
/// Пользователь (таблица asp_net_users)
/// </summary>
public class UserEntity : IdentityUser<Guid>
{
}
