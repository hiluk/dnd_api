using DndSolution.Application.Models.Models.Items;
using DndSolution.Application.Models.Models.Session;

namespace DndSolution.Application.Services.Rules;

/// <summary>
/// Изменения снаряжения и монет. Чистые функции, повторены в приложении
/// и проверяются общим набором примеров.
/// </summary>
public static class InventoryRules
{
    public const int MaxQuantity = 9999;
    public const int MaxCoins = 1_000_000;
    public const int MaxNameLength = 80;
    public const double MaxCustomWeight = 1000;

    public static InventoryState Add(SessionContext context, InventoryState inventory, SessionAction.AddItem action)
    {
        CheckQuantity(action.Quantity);

        if (action.Key is { } key)
        {
            if (!context.Catalog.TryGetValue(key, out var definition))
            {
                throw new SessionRulesException("Key", "Такого предмета нет в справочнике");
            }

            if (definition.Category == ItemCategory.Pack)
            {
                throw new SessionRulesException("Key", "Набор добавляется по содержимому");
            }

            // Такой предмет уже есть — добавляем к нему количеством
            if (inventory.Items.FirstOrDefault(i => i.Key == key) is { } existing)
            {
                var quantity = existing.Quantity + action.Quantity;
                CheckQuantity(quantity);
                return Replace(inventory, existing with { Quantity = quantity });
            }

            return Append(inventory, new InventoryItem { Id = action.ItemId, Key = key, Quantity = action.Quantity });
        }

        var name = action.Name?.Trim();
        if (string.IsNullOrEmpty(name) || name.Length > MaxNameLength)
        {
            throw new SessionRulesException("Name", $"Назовите предмет, до {MaxNameLength} символов");
        }

        if (action.Weight is < 0 or > MaxCustomWeight || action.Weight is double.NaN)
        {
            throw new SessionRulesException("Weight", $"Вес должен быть от 0 до {MaxCustomWeight} фунтов");
        }

        return Append(inventory, new InventoryItem
        {
            Id = action.ItemId,
            Name = name,
            Weight = action.Weight ?? 0,
            Note = string.IsNullOrWhiteSpace(action.Note) ? null : action.Note.Trim(),
            Quantity = action.Quantity,
        });
    }

    public static InventoryState Remove(InventoryState inventory, SessionAction.RemoveItem action)
    {
        var item = Get(inventory, action.ItemId);
        if (action.Quantity < 1 || action.Quantity > item.Quantity)
        {
            throw new SessionRulesException("Quantity", $"Можно убрать от 1 до {item.Quantity}");
        }

        return action.Quantity == item.Quantity
            ? inventory with { Items = inventory.Items.Where(i => i.Id != item.Id).ToList() }
            : Replace(inventory, item with { Quantity = item.Quantity - action.Quantity });
    }

    /// <summary>
    /// Надеть или снять. Новый доспех снимает прежний, новый щит — прежний щит
    /// </summary>
    public static InventoryState SetEquipped(SessionContext context, InventoryState inventory, SessionAction.SetEquipped action)
    {
        var (item, definition) = Find(context, inventory, action.ItemId);
        var category = definition?.Category;
        if (category is not (ItemCategory.Weapon or ItemCategory.Armor or ItemCategory.Shield))
        {
            throw new SessionRulesException("ItemId", "Надеть или взять в руки можно только оружие, доспех или щит");
        }

        var items = inventory.Items.Select(i =>
        {
            if (i.Id == item.Id)
            {
                return i with { Equipped = action.Equipped };
            }

            // Второй доспех или второй щит снимаем
            var sameSlot = action.Equipped && category != ItemCategory.Weapon && i.Equipped
                           && i.Key is { } key && context.Catalog.TryGetValue(key, out var other)
                           && other.Category == category;
            return sameSlot ? i with { Equipped = false } : i;
        }).ToList();

        return inventory with { Items = items };
    }

    public static InventoryState ChangeCurrency(InventoryState inventory, SessionAction.ChangeCurrency action)
    {
        var c = inventory.Currency;
        var next = new Currency(c.Cp + action.Cp, c.Sp + action.Sp, c.Ep + action.Ep, c.Gp + action.Gp, c.Pp + action.Pp);
        int[] coins = [next.Cp, next.Sp, next.Ep, next.Gp, next.Pp];

        if (coins.Any(v => v < 0))
        {
            throw new SessionRulesException("Currency", "Не хватает монет");
        }

        if (coins.Any(v => v > MaxCoins))
        {
            throw new SessionRulesException("Currency", "Слишком много монет");
        }

        return inventory with { Currency = next };
    }

    public static InventoryState UseAmmo(SessionContext context, InventoryState inventory, SessionAction.UseAmmo action)
    {
        var (item, definition) = Find(context, inventory, action.ItemId);
        if (definition?.Category != ItemCategory.Ammunition)
        {
            throw new SessionRulesException("ItemId", "Это не боеприпасы");
        }

        return Remove(inventory, new SessionAction.RemoveItem(item.Id, action.Quantity));
    }

    public static (InventoryItem Item, ItemDefinition? Definition) Find(
        SessionContext context, InventoryState inventory, Guid itemId)
    {
        var item = Get(inventory, itemId);
        var definition = item.Key is { } key && context.Catalog.TryGetValue(key, out var found) ? found : null;
        return (item, definition);
    }

    private static InventoryItem Get(InventoryState inventory, Guid itemId) =>
        inventory.Items.FirstOrDefault(i => i.Id == itemId)
        ?? throw new SessionRulesException("ItemId", "Такого предмета нет в снаряжении");

    private static InventoryState Append(InventoryState inventory, InventoryItem item)
    {
        if (item.Id == Guid.Empty || inventory.Items.Any(i => i.Id == item.Id))
        {
            throw new SessionRulesException("ItemId", "Нужен новый Id предмета");
        }

        return inventory with { Items = [.. inventory.Items, item] };
    }

    private static InventoryState Replace(InventoryState inventory, InventoryItem item) =>
        inventory with { Items = inventory.Items.Select(i => i.Id == item.Id ? item : i).ToList() };

    private static void CheckQuantity(int quantity)
    {
        if (quantity is < 1 or > MaxQuantity)
        {
            throw new SessionRulesException("Quantity", $"Количество должно быть от 1 до {MaxQuantity}");
        }
    }
}
