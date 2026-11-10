using System.Text;
using Core;
using Core.Abstractions;
using Core.Domain;
using Core.Reports;
using Core.Services;
using Core.Storage;

Console.OutputEncoding = Encoding.UTF8;

// Визначення джерела (пам'ять або файл за прапорцем --file)
bool useFile = args.Contains("--file");
string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "catalog.json");

IInventoryStore store = useFile
    ? new FileInventoryStore(dataPath)
    : new InMemoryInventoryStore(SampleData.Items());

var service = new InventoryService(store);
var items = service.GetAll();

// Вибір підкоманди звітів (ігноруємо прапорець --file, якщо він є)
var command = args.FirstOrDefault(a => !a.StartsWith("--")) ?? "report-top";

switch (command)
{
    case "report-top":
        Print(ReportService.TopByQuantity(items, 5));
        break;

    case "report-low":
        Print(ReportService.LowStock(items, 5));
        break;

    case "report-groups":
        foreach (var g in ReportService.ByRarity(items))
        {
            Console.WriteLine($"Рідкість: {g.Key,-10} | позицій: {g.Count,3} | сумарна кількість: {g.TotalQuantity,6}");
        }
        break;

    case "report-sum":
        var t = ReportService.Totals(items);
        Console.WriteLine($"Всього унікальних предметів: {t.Items}, сумарна кількість: {t.TotalQuantity}, середнє: {t.AverageQuantity:F2}, макс в стоках: {t.MaxQuantity}");
        break;

    case "demo-deferred":
        RunDeferredExecutionDemo(items);
        break;

    default:
        Console.WriteLine("Доступні команди: report-top | report-low | report-groups | report-sum | demo-deferred");
        break;
}

// Допоміжний метод для виведення рядків звітів
static void Print(IReadOnlyList<InventoryReportLine> lines)
{
    foreach (var l in lines)
    {
        Console.WriteLine($"{l.Id,-10} {l.ItemName,-20} [{l.Rarity,-8}] Кількість: {l.Quantity,3} | Вартість: {l.BaseValue} золота");
    }
}

//відкладене виконання LINQ-запитів
static void RunDeferredExecutionDemo(IEnumerable<InventoryItem> initialItems)
{
    var source = initialItems.ToList();
    var query = source.Where(item => item.Quantity < 5); // Запит побудовано, але ще не виконано

    Console.WriteLine($"[ДЕМО] Дефіцитних предметів до змін: {query.Count()}");

    // Змінюємо джерело ПІСЛЯ побудови запиту, але ДО його виконання
    var newItem = InventoryItem.Create("TEST01", "Health Potion", "Common", 2, 10.0m, "Small potion");
    source.Add(newItem);

    Console.WriteLine($"[ДЕМО] Дефіцитних предметів після додавання: {query.Count()}"); 

    var snapshot = query.ToList(); // Матеріалізація запиту
    source.Clear();

    Console.WriteLine($"[ДЕМО] У матеріалізованому знімку залишилось: {snapshot.Count}"); 
}