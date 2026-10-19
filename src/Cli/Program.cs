using System.Text;
using Core.Domain;

Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("Сценарій 1: успіх ");
InventoryItem potion = InventoryItem.Create("SLOT-001", "Health Potion", "Common", 10, 12.50m, "Restores 50 HP");
Console.WriteLine(potion);

potion.AddItems(5);
potion.RemoveItems(3);
Console.WriteLine(potion);

Console.WriteLine();
Console.WriteLine("Сценарій 2: порушення інваріантів");
TryDo("витрата більшої кількості, ніж є в інвентарі", () => potion.RemoveItems(100));
TryDo("створення предмета з порожньою назвою", () => InventoryItem.Create("SLOT-002", "", "Rare", 1, 100.0m));
TryDo("від'ємна кількість при створенні", () => InventoryItem.Create("SLOT-003", "Iron Sword", "Common", -5, 150.0m));

static void TryDo(string title, Action action)
{
    try
    {
        action();
        Console.WriteLine($"[!] {title}: виняток НЕ спрацював, інваріант відсутній!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"{title}: {ex.GetType().Name} - {ex.Message}");
    }
}