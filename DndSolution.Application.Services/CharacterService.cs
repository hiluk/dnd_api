using Data.Abstractions;
using DndSolution.Application.Abstractions;
using DndSolution.Application.Models.Enums;
using DndSolution.Application.Models.Models.Backgrounds;
using DndSolution.Application.Models.Models.Character;
using DndSolution.Application.Models.Models.Classes;
using DndSolution.Application.Models.Models.Races;
using DndSolution.Application.Models.Models.Rules;
using DndSolution.Application.Services.Rules;

namespace DndSolution.Application.Services;

/// <summary>
/// Сервис для работы с персонажами
/// </summary>
public class CharacterService : ICharacterService
{
    private readonly ICharactersRepository _repository;
    private readonly IRacesRepository _races;
    private readonly IClassesRepository _classes;
    private readonly IBackgroundsRepository _backgrounds;
    private readonly IAbilityRollService _rolls;

    /// <summary>
    /// .ctor
    /// </summary>
    public CharacterService(
        ICharactersRepository repository,
        IRacesRepository races,
        IClassesRepository classes,
        IBackgroundsRepository backgrounds,
        IAbilityRollService rolls)
    {
        _repository = repository;
        _races = races;
        _classes = classes;
        _backgrounds = backgrounds;
        _rolls = rolls;
    }

    /// <inheritdoc />
    public async Task<CharacterSheet> PreviewAsync(
        Guid userId, CharacterDraft draft, string? rollToken, CancellationToken token)
    {
        var catalog = await LoadCatalogAsync(token);

        return Evaluate(catalog, WithRolls(draft, userId, rollToken));
    }

    /// <inheritdoc />
    public async Task<CharacterWithSheet> CreateCharacterAsync(
        Guid userId, string name, CharacterDraft draft, string? rollToken, CancellationToken token)
    {
        var catalog = await LoadCatalogAsync(token);
        draft = WithRolls(draft, userId, rollToken);
        var sheet = Evaluate(catalog, draft);

        var character = new Character
        {
            Name = name.Trim(),
            Level = CharacterRules.Level,
            CharacterRace = draft.Race,
            CharacterClass = draft.Class,
            GenerationMethod = draft.Method,
            Stats = draft.BaseScores.ToStats(),
            BonusChoices = draft.BonusChoices.ToList(),
            Background = draft.Background,
            BackgroundSkills = draft.BackgroundSkills.ToList(),
            RaceSkills = draft.RaceSkills.ToList(),
            ClassSkills = draft.ClassSkills.ToList(),
            Expertise = draft.Expertise.ToList(),
        };
        await _repository.SaveCharacterAsync(character, userId, token);

        return new CharacterWithSheet(character, sheet);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<CharacterWithSheet>> GetAllUserCharactersAsync(Guid userId, CancellationToken token)
    {
        var characters = await _repository.GetAllUserCharactersAsync(userId, token);
        var catalog = await LoadCatalogAsync(token);

        return characters.Select(c => new CharacterWithSheet(c, Evaluate(catalog, c))).ToList();
    }

    /// <inheritdoc />
    public async Task<CharacterWithSheet?> GetCharacterAsync(Guid id, Guid userId, CancellationToken token)
    {
        var character = await _repository.GetCharacterByIdAsync(userId, id, token);
        if (character is null)
        {
            return null;
        }

        var catalog = await LoadCatalogAsync(token);
        return new CharacterWithSheet(character, Evaluate(catalog, character));
    }

    /// <inheritdoc />
    public Task<bool> DeleteCharacterAsync(Guid id, Guid userId, CancellationToken token)
        => _repository.DeleteCharacterAsync(userId, id, token);

    private CharacterDraft WithRolls(CharacterDraft draft, Guid userId, string? rollToken) =>
        draft.Method == GenerationMethod.Roll
            ? draft with { RolledValues = _rolls.Verify(rollToken, userId) }
            : draft;

    /// <summary>
    /// Сохранённый персонаж уже прошёл проверку бросков при создании,
    /// поэтому при чтении база считается подтверждённым результатом бросков
    /// </summary>
    private static CharacterSheet Evaluate(Catalog catalog, Character character)
    {
        var scores = AbilityScores.From(character.Stats);
        var draft = new CharacterDraft(
            character.CharacterRace, character.CharacterClass, character.GenerationMethod,
            scores, character.BonusChoices, scores.Values.ToList())
        {
            Background = character.Background,
            BackgroundSkills = character.BackgroundSkills,
            RaceSkills = character.RaceSkills,
            ClassSkills = character.ClassSkills,
            Expertise = character.Expertise,
        };

        return Evaluate(catalog, draft);
    }

    private static CharacterSheet Evaluate(Catalog catalog, CharacterDraft draft)
    {
        var race = catalog.Races.FirstOrDefault(r => r.Type == draft.Race);
        var characterClass = catalog.Classes.FirstOrDefault(c => c.Type == draft.Class);
        var background = catalog.Backgrounds.FirstOrDefault(b => b.Type == draft.Background);
        var (field, message) = (race, characterClass, background) switch
        {
            (null, _, _) => ("CharacterRace", "Неизвестная раса"),
            (_, null, _) => ("CharacterClass", "Неизвестный класс"),
            (_, _, null) => ("Background", "Неизвестная предыстория"),
            _ => (null, null),
        };
        if (field is not null)
        {
            throw new CharacterRulesException(new Dictionary<string, string[]> { [field] = [message!] });
        }

        return CharacterRules.Evaluate(draft, race!, characterClass!, background!);
    }

    private async Task<Catalog> LoadCatalogAsync(CancellationToken token) => new(
        await _races.GetAllRaces(token),
        await _classes.GetAllClassesAsync(token),
        await _backgrounds.GetAllAsync(token));

    private sealed record Catalog(
        IReadOnlyList<Race> Races,
        IReadOnlyList<CharacterClass> Classes,
        IReadOnlyList<Background> Backgrounds);
}
