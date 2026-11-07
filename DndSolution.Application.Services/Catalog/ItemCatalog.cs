using System.Reflection;
using System.Text.Json;
using DndSolution.Application.Models.Models.Items;

namespace DndSolution.Application.Services.Catalog;

/// <summary>
/// Справочник предметов из встроенного items.json (SRD 5.1)
/// </summary>
public static class ItemCatalog
{
    private static readonly Lazy<IReadOnlyDictionary<string, ItemDefinition>> Items = new(Load);

    public static IReadOnlyDictionary<string, ItemDefinition> All => Items.Value;

    /// <summary>
    /// Справочник из JSON: тот же формат читают тесты сервера и клиента
    /// </summary>
    public static IReadOnlyDictionary<string, ItemDefinition> Parse(string json)
    {
        var root = JsonSerializer.Deserialize<CatalogFile>(json, new JsonSerializerOptions(JsonSerializerDefaults.Web))!;
        return root.Items.ToDictionary(i => i.Key);
    }

    private static IReadOnlyDictionary<string, ItemDefinition> Load()
    {
        using var stream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("DndSolution.Application.Services.Catalog.items.json")!;
        using var reader = new StreamReader(stream);
        return Parse(reader.ReadToEnd());
    }

    private sealed record CatalogFile(IReadOnlyList<ItemDefinition> Items);
}
