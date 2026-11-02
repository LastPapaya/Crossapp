using Core.Abstractions;
using Core.Domain;
using Core.Events;

namespace Core.Services;

public sealed class InventoryService(IInventoryStore store)
{
    private readonly IInventoryStore _store = store ?? throw new ArgumentNullException(nameof(store));

    public event EventHandler<CatalogChangedEventArgs>? Changed;

    private void OnChanged(InventoryItem item, CatalogChangeKind kind, int delta) =>
        Changed?.Invoke(this, new CatalogChangedEventArgs(item.Id, kind, delta, item.Quantity));

    public InventoryItem AddItem(string itemName, string rarity, int quantity, decimal baseValue, string? note = null)
    {
        var id = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        var item = InventoryItem.Create(id, itemName, rarity, quantity, baseValue, note);
        
        _store.Add(item);
        OnChanged(item, CatalogChangeKind.Added, quantity); 
        
        return item;
    }

    public void AddQuantity(string id, int amount)
    {
        var item = _store.GetById(id) ?? throw new InvalidOperationException($"Немає предмета з id={id}.");
        
        item.AddItems(amount); 
        _store.Update(item);
        
        OnChanged(item, CatalogChangeKind.Received, amount);
    }

    public void RemoveQuantity(string id, int amount)
    {
        var item = _store.GetById(id) ?? throw new InvalidOperationException($"Немає предмета з id={id}.");
        
        item.RemoveItems(amount); 
        _store.Update(item);
        
        OnChanged(item, CatalogChangeKind.Issued, amount);
    }

    public IReadOnlyList<InventoryItem> GetAll() => _store.List();

    public InventoryItem? FindById(string id) => _store.GetById(id);

    
    public IReadOnlyList<InventoryItem> FindAll(Func<InventoryItem, bool> predicate) =>
        _store.List().Where(predicate).ToList();
}