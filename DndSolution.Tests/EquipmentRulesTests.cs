using System.Text.Json;
using System.Text.Json.Nodes;
using DndSolution.Application.Models.Models.Rules;
using DndSolution.Application.Models.Models.Session;
using DndSolution.Application.Services.Catalog;
using DndSolution.Application.Services.Rules;

namespace DndSolution.Tests;

/// <summary>
/// КД, скорость, вес и атаки по общему набору примеров docs/equipment-rules-cases.json.
/// Тот же файл проверяет приложение.
/// </summary>
public class EquipmentRulesTests
{
    private static readonly JsonArray Cases =
        JsonNode.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "equipment-rules-cases.json")))!
            ["cases"]!.AsArray();

    public static IEnumerable<object[]> CaseNames() => Cases.Select(c => new object[] { c!["name"]!.GetValue<string>() });

    [Theory]
    [MemberData(nameof(CaseNames))]
    public void Пример(string name)
    {
        var data = Cases.Single(c => c!["name"]!.GetValue<string>() == name)!;
        var context = new SessionContext(10, 8, 1, 0)
        {
            Catalog = ItemCatalog.All,
            Equipment = Profile(data["profile"]!),
        };

        var summary = EquipmentRules.Evaluate(context, SessionRulesTests.Inventory(data["inventory"]!));
        var expect = data["expect"]!;

        Assert.Equal(expect["armorClass"]!.GetValue<int>(), summary.ArmorClass);
        Assert.Equal(expect["armorSource"]!.GetValue<string>(), summary.ArmorSource);
        Assert.Equal(expect["speed"]!.GetValue<int>(), summary.Speed);
        Assert.Equal(expect["stealthDisadvantage"]!.GetValue<bool>(), summary.StealthDisadvantage);
        Assert.Equal(expect["weight"]!.GetValue<double>(), summary.Weight);
        Assert.Equal(expect["capacity"]!.GetValue<int>(), summary.Capacity);
        Assert.Equal(expect["encumbered"]!.GetValue<bool>(), summary.Encumbered);
        Assert.Equal(expect["warnings"]!.AsArray().Select(w => w!.GetValue<string>()), summary.Warnings);

        var attacks = JsonSerializer.SerializeToNode(summary.Attacks, new JsonSerializerOptions(JsonSerializerDefaults.Web))!;
        Assert.True(
            JsonNode.DeepEquals(expect["attacks"], attacks),
            $"Ожидали {expect["attacks"]!.ToJsonString()}, получили {attacks.ToJsonString()}");
    }

    private static EquipmentProfile Profile(JsonNode node)
    {
        static AbilityScores Abilities(JsonNode n) => new(
            n["strength"]!.GetValue<int>(), n["dexterity"]!.GetValue<int>(), n["constitution"]!.GetValue<int>(),
            n["intelligence"]!.GetValue<int>(), n["wisdom"]!.GetValue<int>(), n["charisma"]!.GetValue<int>());

        static List<string> Strings(JsonNode n) => n.AsArray().Select(s => s!.GetValue<string>()).ToList();

        return new EquipmentProfile(
            Abilities(node["scores"]!),
            Abilities(node["modifiers"]!),
            node["proficiencyBonus"]!.GetValue<int>(),
            node["baseSpeed"]!.GetValue<int>(),
            Enum.Parse<UnarmoredDefense>(node["unarmored"]!.GetValue<string>(), ignoreCase: true),
            Strings(node["armorProficiencies"]!),
            Strings(node["weaponProficiencies"]!),
            node["ignoresHeavyArmorSpeed"]!.GetValue<bool>());
    }
}
