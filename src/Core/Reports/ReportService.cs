using Core.Domain;

namespace Core.Reports;

public static class ReportService
{
    // 1) Фільтр за дефіцитом + стабільне сортування + проєкція в DTO
    public static IReadOnlyList<InventoryReportLine> LowStock(IEnumerable<InventoryItem> source, int threshold) =>
        source.Where(item => item.Quantity < threshold)
              .OrderBy(item => item.Quantity)
              .ThenBy(item => item.ItemName, StringComparer.Ordinal)
              .Select(item => new InventoryReportLine(item.Id, item.ItemName, item.Rarity, item.Quantity, item.BaseValue))
              .ToList();

    // 2) Топ-N найчисленніших предметів зі стабільним порядком
    public static IReadOnlyList<InventoryReportLine> TopByQuantity(IEnumerable<InventoryItem> source, int n) =>
        source.OrderByDescending(item => item.Quantity)
              .ThenBy(item => item.ItemName, StringComparer.Ordinal)
              .Take(n)
              .Select(item => new InventoryReportLine(item.Id, item.ItemName, item.Rarity, item.Quantity, item.BaseValue))
              .ToList();

    // 3) Групування за рідкістю
    public static IReadOnlyList<GroupSummary> ByRarity(IEnumerable<InventoryItem> source) =>
        source.GroupBy(item => item.Rarity, StringComparer.OrdinalIgnoreCase)
              .Select(g => new GroupSummary(g.Key, g.Count(), g.Sum(item => item.Quantity)))
              .OrderByDescending(s => s.TotalQuantity)
              .ToList();

    // 4) Агрегати з безпечною обробкою порожньої послідовності (іменований кортеж)
    public static (int Items, int TotalQuantity, double AverageQuantity, int MaxQuantity) Totals(IEnumerable<InventoryItem> source)
    {
        var list = source.ToList();
        return list.Count == 0
            ? (0, 0, 0d, 0)
            : (list.Count, 
               list.Sum(item => item.Quantity), 
               list.Average(item => item.Quantity), 
               list.Max(item => item.Quantity));
    }
}
