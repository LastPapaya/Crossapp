using System.Text.Json;
using Core.Abstractions;
using Core.Domain;
using Core.Dto;

namespace Core.Storage;

public sealed class JsonCatalogImporter : ICatalogImporter
{
    public async Task<IReadOnlyList<InventoryItem>> LoadAsync(string path, CancellationToken ct = default)
    {
        //асинхронне читання файлу без блокування потоку (useAsync: true)
        await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
        
        var dtos = await JsonSerializer.DeserializeAsync<List<InventoryItemDto>>(stream, cancellationToken: ct)
            ?? throw new DomainException($"Файл {Path.GetFileName(path)} не містить списку предметів.");

        var result = new List<InventoryItem>(dtos.Count);
        for (var i = 0; i < dtos.Count; i++)
        {
            //перевірка скасування на кожній ітерації циклу
            ct.ThrowIfCancellationRequested();
            try 
            { 
                result.Add(InventoryItem.FromDto(dtos[i])); 
            }
            catch (ArgumentException ex) 
            { 
                throw new DomainException($"Запис {i + 1}: {ex.Message}", ex); 
            }
        }
        return result;
    }
}