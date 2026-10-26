using Core.Abstractions;
using Core.Domain;

namespace Core.Services;

public sealed class InventoryService(IInventoryStore store)
{
    private readonly IInventoryStore _store = store ?? throw new ArgumentNullException(nameof(store));

    public InventoryItem AddItem(string itemName, string rarity, int quantity, decimal baseValue, string? note = null)
    {
        var id = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        var item = InventoryItem.Create(id, itemName, rarity, quantity, baseValue, note);
        _store.Add(item);
        return item;
    }

    public void AddQuantity(string id, int amount)
    {
        var item = _store.GetById(id) ?? throw new InvalidOperationException($"Немає предмета з id={id}.");
        item.AddItems(amount);
        _store.Update(item);
    }

    public void RemoveQuantity(string id, int amount)
    {
        var item = _store.GetById(id) ?? throw new InvalidOperationException($"Немає предмета з id={id}.");
        item.RemoveItems(amount);
        _store.Update(item);
    }

    public IReadOnlyList<InventoryItem> GetAll() => _store.List();

    public InventoryItem? FindById(string id) => _store.GetById(id);
}