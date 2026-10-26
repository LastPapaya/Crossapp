using Core.Domain;

namespace Core.Abstractions;

public interface IInventoryStore
{
    IReadOnlyList<InventoryItem> List();
    InventoryItem? GetById(string id);
    void Add(InventoryItem item);
    void Update(InventoryItem item);
    bool Remove(string id);
}