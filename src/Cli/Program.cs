using System.Text;
using Core;
using Core.Abstractions;
using Core.Events;
using Core.Services;
using Core.Storage;

Console.OutputEncoding = Encoding.UTF8;

bool useFile = args.Contains("--file");
string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "catalog.json");

IInventoryStore store = useFile
    ? new FileInventoryStore(dataPath)
    : new InMemoryInventoryStore(SampleData.Items());

var service = new InventoryService(store);


var logPath = Path.Combine(AppContext.BaseDirectory, "logs", "app.log");
Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

void OnCatalogChanged(object? sender, CatalogChangedEventArgs e)
{
    var line = $"{e.At:HH:mm:ss} {e.Kind,-8} {e.Id} {e.Delta,4:+0;-0} → залишок {e.QuantityAfter}";
    Console.WriteLine(line);
    File.AppendAllText(logPath, line + Environment.NewLine);
}


service.Changed += OnCatalogChanged;


int changes = 0;
service.Changed += (_, _) => changes++;

Console.WriteLine($"[INFO] Сховище: {store.GetType().Name}");
Console.WriteLine();


Console.WriteLine("Сценарій: робота з подіями");
var created = service.AddItem("Magic Shield", "Rare", 2, 250.0m, "Sturdy shield");
service.AddQuantity(created.Id, 3);

Console.WriteLine($"Подій отримано (лічильник): {changes}");


service.Changed -= OnCatalogChanged;


service.AddQuantity(created.Id, 5); 
Console.WriteLine($"Подій отримано після відписки (тільки лямбда): {changes}");

Console.WriteLine();


Console.WriteLine("Сценарій відмови");
TryDo("витрата більшої кількості, ніж є в інвентарі", () => service.RemoveQuantity(created.Id, 1000));


Console.WriteLine();
Console.WriteLine("Пошук через Func (предмети з кількістю < 10)");
var lowStockItems = service.FindAll(p => p.Quantity < 10);
foreach (var item in lowStockItems)
{
    Console.WriteLine($"Знайдено дефіцитний предмет: {item.ItemName} (Кількість: {item.Quantity})");
}

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