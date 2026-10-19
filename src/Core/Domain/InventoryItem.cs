using Core.Dto;

namespace Core.Domain;

public sealed class InventoryItem
{
    private int _quantity;
    private decimal _baseValue;

    public string Id { get; }
    public string ItemName { get; }
    public string Rarity { get; }
    public int Quantity => _quantity;
    public decimal BaseValue => _baseValue;
    public string? Note { get; }

    private InventoryItem(string id, string itemName, string rarity, int quantity, decimal baseValue, string? note)
    {
        Id = id;
        ItemName = itemName;
        Rarity = rarity;
        _quantity = quantity;
        _baseValue = baseValue;
        Note = note;
    }

    public static InventoryItem Create(string id, string itemName, string rarity, int quantity, decimal baseValue, string? note = null)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Ідентифікатор слоту обов'язковий", nameof(id));
        if (string.IsNullOrWhiteSpace(itemName))
            throw new ArgumentException("Назва предмета не може бути порожньою", nameof(itemName));
        if (string.IsNullOrWhiteSpace(rarity))
            throw new ArgumentException("Рідкісність предмета обов'язкова", nameof(rarity));
        if (quantity < 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), quantity, "Кількість предметів не може бути від'ємною");
        if (baseValue < 0)
            throw new ArgumentOutOfRangeException(nameof(baseValue), baseValue, "Базова вартість не може бути від'ємною");

        return new InventoryItem(
            id.Trim(),
            itemName.Trim(),
            rarity.Trim(),
            quantity,
            baseValue,
            string.IsNullOrWhiteSpace(note) ? null : note.Trim()
        );
    }

    public void AddItems(int amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), amount, "Кількість для додавання має бути більшою за нуль");

        _quantity += amount;
    }

    public void RemoveItems(int amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), amount, "Кількість для видалення має бути більшою за нуль");

        if (amount > _quantity)
            throw new InvalidOperationException($"Неможливо видалити {amount} од. предмета '{ItemName}': у інвентарі є лише {_quantity}");

        _quantity -= amount;
    }

    public InventoryItemDto ToDto() => new(Id, ItemName, Rarity, Quantity, BaseValue, Note);

    public static InventoryItem FromDto(InventoryItemDto dto) =>
        Create(dto.Id, dto.ItemName, dto.Rarity, dto.Quantity, dto.BaseValue, dto.Note);

    public override string ToString() => $"{Id,-9} [{Rarity}] {ItemName} — {_quantity} шт. ({_baseValue} gold)";
}