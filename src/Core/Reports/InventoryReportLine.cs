namespace Core.Reports;

public sealed record InventoryReportLine(string Id, string ItemName, string Rarity, int Quantity, decimal BaseValue);