using Core;
using Core.Abstractions;
using Core.Domain;
using Core.Events;
using Core.Reports;
using Core.Services;
using Core.Storage;
using Xunit;

public class InventoryServiceTests
{
    [Fact]
    // 1) Інваріант: відмова і незмінний стан при помилці
    public void AddQuantity_WithNegativeAmount_Throws_AndKeepsState()
    {
        var store = new InMemoryInventoryStore();
        var service = new InventoryService(store);
        
        
        var item = service.AddItem("Меч", "Common", 10, 100m, "Test");

        
        Assert.Throws<ArgumentException>(() => service.AddQuantity(item.Id, -5));
        
        
        Assert.Equal(10, store.GetById(item.Id)!.Quantity);
    }

    [Fact]
    // 2) Перевірка події  піднімається рівно один раз
    public void AddItem_RaisesChangedExactlyOnce()
    {
        var service = new InventoryService(new InMemoryInventoryStore());
        var events = new List<CatalogChangedEventArgs>();
        service.Changed += (_, e) => events.Add(e);

        
        service.AddItem("Щит", "Epic", 7, 250m, "Test");

        Assert.Single(events);
        Assert.Equal(CatalogChangeKind.Added, events[0].Kind);
    }

    [Theory]
    [InlineData(10, 2)]
    [InlineData(2, 0)]
    // 3) Звіт LowStock на кількох наборах даних
    public void LowStock_CountsOnlyBelowThreshold(int threshold, int expected)
    {
        var items = new[]
        {
            InventoryItem.Create("1", "A", "Common", 1, 10m, ""),
            InventoryItem.Create("2", "B", "Common", 5, 10m, ""),
            InventoryItem.Create("3", "C", "Common", 15, 10m, "")
        };
        var result = ReportService.LowStock(items, threshold);
        Assert.Equal(expected, result.Count);
    }

    [Fact]
    // 4) Детермінований топ-N
    public void TopByQuantity_IsDeterministic()
    {
        var items = new[]
        {
            InventoryItem.Create("1", "Apple", "Common", 10, 1m, ""),
            InventoryItem.Create("2", "Banana", "Common", 10, 1m, ""),
            InventoryItem.Create("3", "Sword", "Rare", 50, 100m, "")
        };
        var top = ReportService.TopByQuantity(items, 2);
        Assert.Equal("1", top[1].Id); 
    }

    [Fact]
    // 5) Скасування асинхронної операції
    public async Task ImportAsync_WhenAlreadyCancelled_Throws()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var service = new InventoryService(new InMemoryInventoryStore());
        
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            await service.ImportAsync(new JsonCatalogImporter(), "any.json", cts.Token));
    }

    [Fact]
    // 6) Додатковий тест: зіпсований файл викликає DomainException
    public async Task ImportAsync_WithInvalidData_ThrowsDomainException()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            await File.WriteAllTextAsync(tempFile, "[{\"Id\":\"\",\"ItemName\":\"Bad\",\"Quantity\":-5}]");

            var service = new InventoryService(new InMemoryInventoryStore());
            
            await Assert.ThrowsAsync<DomainException>(async () =>
                await service.ImportAsync(new JsonCatalogImporter(), tempFile));
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }
}