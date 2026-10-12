namespace Core.Dto;

public record InventoryItemDto(
    string Id,
    string ItemName,
    string Rarity,
    int Quantity,
    decimal BaseValue,
    string? Note = null
);