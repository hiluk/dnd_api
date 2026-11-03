using System.Security.Cryptography;
using DndSolution.Application.Abstractions;
using DndSolution.Application.Models.Models.Rules;
using Microsoft.AspNetCore.DataProtection;

namespace DndSolution.Application.Services;

/// <summary>
/// Бросает кубики и подписывает результат, чтобы его нельзя было подменить на клиенте.
/// Результаты не хранятся: подпись содержит значения, Id пользователя и срок действия.
/// </summary>
public class AbilityRollService : IAbilityRollService
{
    public static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(1);

    private readonly ITimeLimitedDataProtector _protector;

    public AbilityRollService(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector("DndSolution.AbilityRoll.v1").ToTimeLimitedDataProtector();
    }

    /// <inheritdoc />
    public AbilityRoll Roll(Guid userId)
    {
        var values = Enumerable.Range(0, 6).Select(_ => RollAbility()).ToList();
        var token = _protector.Protect($"{userId}:{string.Join(',', values)}", TokenLifetime);

        return new AbilityRoll(values, token);
    }

    /// <inheritdoc />
    public IReadOnlyList<int>? Verify(string? token, Guid userId)
    {
        if (string.IsNullOrEmpty(token))
        {
            return null;
        }

        try
        {
            var payload = _protector.Unprotect(token).Split(':');
            if (payload.Length != 2 || payload[0] != userId.ToString())
            {
                return null;
            }

            return payload[1].Split(',').Select(int.Parse).ToList();
        }
        catch (CryptographicException)
        {
            return null;
        }
    }

    /// <summary>
    /// 4к6, наименьший результат отбрасывается
    /// </summary>
    private static int RollAbility()
    {
        var dice = Enumerable.Range(0, 4).Select(_ => RandomNumberGenerator.GetInt32(1, 7)).ToList();

        return dice.Sum() - dice.Min();
    }
}
