using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;


Console.OutputEncoding = Encoding.UTF8;


var envInfo = new
{
    Application = "CrossApp – Game Inventory ",
    Student = "Chukh Roman, FEI-33", 
    OSDescription = RuntimeInformation.OSDescription,
    OSVersion = Environment.OSVersion.ToString(),
    ProcessArchitecture = RuntimeInformation.ProcessArchitecture.ToString(),
    DotNetVersion = Environment.Version.ToString(),
    Runtime = RuntimeInformation.FrameworkDescription,
    BaseDirectory = AppContext.BaseDirectory,
    CurrentDirectory = Environment.CurrentDirectory,
    Domain = "Ігровий інвентар та торгівля (Player, Item, InventorySlot, TradeTransaction)"
};


if (args.Length > 0 && args[0].Equals("--json", StringComparison.OrdinalIgnoreCase))
{
    string jsonOutput = JsonSerializer.Serialize(envInfo, new JsonSerializerOptions { WriteIndented = true });
    Console.WriteLine(jsonOutput);
    return;
}


Console.WriteLine(envInfo.Application);
Console.WriteLine($"Студент: {envInfo.Student}");
Console.WriteLine(new string('-', 60));
Console.WriteLine($"ОС (OSDescription)   : {envInfo.OSDescription}");
Console.WriteLine($"ОС (Environment)     : {envInfo.OSVersion}");
Console.WriteLine($"Архітектура процесу  : {envInfo.ProcessArchitecture}");
Console.WriteLine($"Версія .NET (CLR)    : {envInfo.DotNetVersion}");
Console.WriteLine($"Runtime              : {envInfo.Runtime}");
Console.WriteLine($"Каталог застосунку   : {envInfo.BaseDirectory}");
Console.WriteLine($"Поточний каталог     : {envInfo.CurrentDirectory}");
Console.WriteLine(new string('-', 60));
Console.WriteLine($"Предметна область    : {envInfo.Domain}");