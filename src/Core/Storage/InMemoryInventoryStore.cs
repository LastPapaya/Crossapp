using Core.Abstractions;
using Core.Domain;

namespace Core.Storage;

public sealed class InMemoryInventoryStore(IEnumerable<InventoryItem>? seed = null) : IInventoryStore
{
    private readonly Dictionary<string, InventoryItem> _items =
        (seed ?? []).ToDictionary(p => p.Id, StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<InventoryItem> List() => _items.Values.ToList();

    public InventoryItem? GetById(string id) => _items.GetValueOrDefault(id);

    public void Add(InventoryItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (_items.ContainsKey(item.Id))
            throw new InvalidOperationException($"Запис з id={item.Id} уже існує.");
        
        _items.Add(item.Id, item);
    }

    public void Update(InventoryItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _items[item.Id] = item;
    }

    public bool Remove(string id)
    {
        return _items.Remove(id);
    }
}