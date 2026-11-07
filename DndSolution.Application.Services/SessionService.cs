using Data.Abstractions;
using DndSolution.Application.Abstractions;
using DndSolution.Application.Models.Models.Session;
using DndSolution.Application.Services.Catalog;
using DndSolution.Application.Services.Rules;

namespace DndSolution.Application.Services;

public sealed class SessionService : ISessionService
{
    /// <summary>
    /// Сколько раз перечитать состояние, если его изменили одновременно
    /// </summary>
    private const int Attempts = 3;

    private readonly ICharacterService _characters;
    private readonly IClassesRepository _classes;
    private readonly ISessionRepository _sessions;

    public SessionService(ICharacterService characters, IClassesRepository classes, ISessionRepository sessions)
    {
        _characters = characters;
        _classes = classes;
        _sessions = sessions;
    }

    public async Task<SessionView?> GetAsync(Guid userId, Guid characterId, CancellationToken token)
    {
        var context = await ContextAsync(userId, characterId, token);
        if (context is null)
        {
            return null;
        }

        var state = await _sessions.GetStateAsync(characterId, token) ?? SessionState.Fresh(context);
        return new SessionView(state, context);
    }

    public async Task<(SessionView View, SessionEvent Event)?> ApplyAsync(
        Guid userId, Guid characterId, Guid actionId, SessionAction action, DateTime createdAt, CancellationToken token)
    {
        var context = await ContextAsync(userId, characterId, token);
        if (context is null)
        {
            return null;
        }

        for (var attempt = 0; attempt < Attempts; attempt++)
        {
            // Повтор из очереди клиента: действие уже применено
            if (await _sessions.GetEventAsync(characterId, actionId, token) is { } existing)
            {
                return (await CurrentAsync(characterId, context, token), existing);
            }

            var saved = await _sessions.GetStateAsync(characterId, token);
            var state = saved ?? SessionState.Fresh(context);
            var result = SessionRules.Apply(context, state, action);
            var sessionEvent = new SessionEvent(actionId, action, result, Normalize(createdAt));

            if (await _sessions.SaveAsync(characterId, saved, sessionEvent, token))
            {
                return (new SessionView(result.State, context), sessionEvent);
            }
        }

        throw new InvalidOperationException("Состояние сессии меняется слишком часто, повторите действие");
    }

    public async Task<IReadOnlyList<SessionEvent>?> GetEventsAsync(
        Guid userId, Guid characterId, DateTime? before, int limit, CancellationToken token)
    {
        if (await ContextAsync(userId, characterId, token) is null)
        {
            return null;
        }

        return await _sessions.GetEventsAsync(characterId, before, Math.Clamp(limit, 1, 200), token);
    }

    private async Task<SessionView> CurrentAsync(Guid characterId, SessionContext context, CancellationToken token) =>
        new(await _sessions.GetStateAsync(characterId, token) ?? SessionState.Fresh(context), context);

    /// <summary>
    /// Максимум хитов, кость хитов и Телосложение берутся из листа персонажа
    /// </summary>
    private async Task<SessionContext?> ContextAsync(Guid userId, Guid characterId, CancellationToken token)
    {
        var found = await _characters.GetCharacterAsync(characterId, userId, token);
        if (found is null)
        {
            return null;
        }

        var (character, sheet) = found;
        var classes = await _classes.GetAllClassesAsync(token);
        var hitDie = classes.First(c => c.Type == character.CharacterClass).HitDie;
        var catalog = ItemCatalog.All;

        return new SessionContext(sheet.HitPoints, hitDie, character.Level, sheet.Modifiers.Constitution)
        {
            Catalog = catalog,
            Equipment = new EquipmentProfile(
                sheet.Scores,
                sheet.Modifiers,
                sheet.ProficiencyBonus,
                sheet.Speed,
                Proficiencies.Unarmored(character.CharacterClass),
                Proficiencies.Armor(character.CharacterClass),
                Proficiencies.Weapons(character.CharacterClass, character.CharacterRace, catalog),
                Proficiencies.IgnoresHeavyArmorSpeed(character.CharacterRace)),
        };
    }

    /// <summary>
    /// Время с устройства: без часового пояса считаем UTC, из будущего не принимаем
    /// </summary>
    private static DateTime Normalize(DateTime createdAt)
    {
        var utc = createdAt.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(createdAt, DateTimeKind.Utc)
            : createdAt.ToUniversalTime();
        var now = DateTime.UtcNow;

        return utc == default || utc > now ? now : utc;
    }
}
