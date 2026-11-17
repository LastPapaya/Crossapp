using Core.Domain;

namespace Core.Abstractions;

public interface ICatalogImporter
{
    Task<IReadOnlyList<InventoryItem>> LoadAsync(string path, CancellationToken ct = default);
}