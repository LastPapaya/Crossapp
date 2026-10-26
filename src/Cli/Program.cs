using System.Text;
using Core;
using Core.Abstractions;
using Core.Services;
using Core.Storage;

Console.OutputEncoding = Encoding.UTF8;

bool useFile = args.Contains("--file");
string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "catalog.json");

IInventoryStore store = useFile
    ? new FileInventoryStore(dataPath)
    : new InMemoryInventoryStore(SampleData.Items());

var service = new InventoryService(store);

Console.WriteLine($"[INFO] Сховище: {store.GetType().Name}");
Console.WriteLine();


Console.WriteLine("Перегляд та додавання");
var created = service.AddItem("Mana Potion", "Common", 5, 20.0m, "Restores MP");
service.AddQuantity(created.Id, 10);

foreach (var item in service.GetAll())
{
    Console.WriteLine(item);
}


Console.WriteLine();
Console.WriteLine("Відмова");
TryDo("видалення неіснуючого ID", () => service.RemoveQuantity("NOT-FOUND", 5));

static void TryDo(string title, Action action)
{
    try
    {
        action();
        Console.WriteLine($"[!] {title}: виняток НЕ спрацював!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"{title}: {ex.GetType().Name} - {ex.Message}");
    }
}