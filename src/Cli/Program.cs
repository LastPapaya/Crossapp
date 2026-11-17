using System.Text;
using Core;
using Core.Abstractions;
using Core.Services;
using Core.Storage;

Console.OutputEncoding = Encoding.UTF8;

using var cts = new CancellationTokenSource();

Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };



bool useFile = args.Contains("--file");
string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "catalog.json");

IInventoryStore store = useFile
    ? new FileInventoryStore(dataPath)
    : new InMemoryInventoryStore(SampleData.Items());

var service = new InventoryService(store);

try
{
    
    if (args.Contains("import"))
    {
        var added = await service.ImportAsync(new JsonCatalogImporter(), dataPath, cts.Token);
        Console.WriteLine($"Імпортовано записів: {added}");
    }
    else
    {
        Console.WriteLine("Використовуйте команду import для асинхронного завантаження або інші звіти.");
    }
}
catch (OperationCanceledException) 
{ 
    Console.WriteLine("Імпорт скасовано."); 
}
catch (DomainException ex) 
{ 
    Console.WriteLine($"Дані відхилено: {ex.Message}"); 
}
catch (FileNotFoundException) 
{ 
    Console.WriteLine($"Файл не знайдено: {dataPath}"); 
}