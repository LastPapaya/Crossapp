using System.Globalization;
using Core.Dto;

namespace Core.Import;

public static class InventoryCsvImporter
{
    private const char Separator = ';';

    public static ImportResult<InventoryItemDto> Load(string path)
    {
        var items = new List<InventoryItemDto>();
        var errors = new List<string>();

        string[] lines = File.ReadAllLines(path);

        for (int i = 0; i < lines.Length; i++)
        {
            int lineNumber = i + 1;
            string line = lines[i];

            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                continue;

            if (lineNumber == 1 && line.StartsWith("id", StringComparison.OrdinalIgnoreCase))
                continue;

            switch (ParseLine(line))
            {
                case ParseOk ok:
                    items.Add(ok.Value);
                    break;
                case ParseFailed failed:
                    errors.Add($"рядок {lineNumber}: {failed.Reason}");
                    break;
            }
        }

        return new ImportResult<InventoryItemDto>(items, errors);
    }

    private static ParseOutcome ParseLine(string line)
    {
        string[] parts = line.Split(Separator, StringSplitOptions.TrimEntries);

        return parts switch
        {
            { Length: < 5 } => 
                new ParseFailed($"очікую щонайменше 5 колонок, отримано {parts.Length}"),

            [_, "", _, _, _, ..] or [_, _, "", _, _, ..] => 
                new ParseFailed("ID або Назва предмета не можуть бути порожніми"),

            [_, _, _, var qtyStr, _, ..] when !int.TryParse(qtyStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out int q) || q < 0 => 
                new ParseFailed($"кількість '{qtyStr}' не є правильним невід'ємним цілим числом"),

            [_, _, _, _, var priceStr, ..] when !decimal.TryParse(priceStr, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal price) || price < 0 => 
                new ParseFailed($"ціна '{priceStr}' не є правильним числом або є від'ємною"),

            [var id, var name, var rarity, var qtyStr, var priceStr] => 
                new ParseOk(new InventoryItemDto(id, name, rarity, int.Parse(qtyStr, CultureInfo.InvariantCulture), decimal.Parse(priceStr, CultureInfo.InvariantCulture))),

            [var id, var name, var rarity, var qtyStr, var priceStr, var note] => 
                new ParseOk(new InventoryItemDto(id, name, rarity, int.Parse(qtyStr, CultureInfo.InvariantCulture), decimal.Parse(priceStr, CultureInfo.InvariantCulture), string.IsNullOrWhiteSpace(note) ? null : note)),

            _ => new ParseFailed($"занадто багато колонок: {parts.Length}")
        };
    }

    private abstract record ParseOutcome;
    private sealed record ParseOk(InventoryItemDto Value) : ParseOutcome;
    private sealed record ParseFailed(string Reason) : ParseOutcome;
}