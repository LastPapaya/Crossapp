using Core.Domain;

namespace Core;

public static class SampleData
{
    public static IReadOnlyList<InventoryItem> Items() =>
    [
        InventoryItem.Create("ITM-001", "Health Potion", "Common", 10, 15.0m, "Restores HP"),
        InventoryItem.Create("ITM-002", "Iron Sword", "Common", 1, 150.0m, "Basic weapon"),
        InventoryItem.Create("ITM-003", "Magic Wand", "Rare", 2, 450.0m, "Mana boost")
    ];
}