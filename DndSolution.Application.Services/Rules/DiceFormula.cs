using System.Text.RegularExpressions;

namespace DndSolution.Application.Services.Rules;

/// <summary>
/// Формула броска: «2d4+2», «1d8», «1». В интерфейсе пишется по-русски: «2к4 + 2»
/// </summary>
public sealed partial record DiceFormula(int Count, int Faces, int Bonus)
{
    public int Min => Count + Bonus;

    public int Max => Count * Faces + Bonus;

    public static DiceFormula Parse(string formula)
    {
        var match = Pattern().Match(formula.Replace(" ", string.Empty));
        if (!match.Success)
        {
            throw new FormatException($"Неверная формула броска: {formula}");
        }

        if (!match.Groups["faces"].Success)
        {
            // Постоянное значение, например урон духовой трубки
            return new DiceFormula(0, 0, int.Parse(match.Groups["count"].Value));
        }

        var bonus = match.Groups["bonus"].Success ? int.Parse(match.Groups["bonus"].Value) : 0;
        return new DiceFormula(int.Parse(match.Groups["count"].Value), int.Parse(match.Groups["faces"].Value), bonus);
    }

    /// <summary>
    /// «2к4 + 2»
    /// </summary>
    public override string ToString()
    {
        var dice = Faces == 0 ? $"{Bonus}" : $"{Count}к{Faces}";
        return Faces == 0 || Bonus == 0 ? dice : $"{dice} {(Bonus > 0 ? "+" : "−")} {Math.Abs(Bonus)}";
    }

    [GeneratedRegex(@"^(?<count>\d+)(?:d(?<faces>\d+)(?<bonus>[+-]\d+)?)?$")]
    private static partial Regex Pattern();
}
