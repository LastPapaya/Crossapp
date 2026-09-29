# CrossApp — Multi-project Solution (.NET 9)

## Предметна область
**Ігровий інвентар та торгівля**
* **Сутності:** 
  * `Player` (Гравець) — нікнейм, рівень, внутрішньоігровий баланс.
  * `Item` (Предмет) — назва, рідкісність, базова цінність, атрибути.
  * `InventorySlot` (Слот інвентарю) — прив'язка предмета до гравця, кількість, стан.
  * `TradeTransaction` (Угода торгівлі) — продавець, покупець, предмет, ціна, дата транзакції.
* **Призначення:** Система для управління персональними інвентарями гравців, обліку ігрових предметів та проведення внутрішньоігрових торговельних угод.



## Структура рішення (Lab 02)
CrossApp/
├── CrossApp.sln
├── README.md
├── .gitignore
└── src/
├── Core/
│   ├── Core.csproj
│   └── EnvironmentInfo.cs
└── Cli/
├── Cli.csproj
└── Program.cs


### Домовленість про структуру папок у Core:
* `Core/Dto/` — record-типи для формату даних
* `Core/Domain/` — сутності з бізнес-логікою та інваріантами
* `Core/Storage/` — реалізації сховищ даних



## Порівняння режимів публікації (Lab 02)

| RID | Режим публікації | Розмір publish | Потрібен встановлений runtime |
| :--- | :--- | :--- | :--- |
| **win-x64** | Self-contained | ~70 - 80 МБ | Ні |
| **win-x64** | Framework-dependent | ~0.2 - 0.5 МБ | Так (.NET 9 Runtime) |

## Інструкція з запуску та збірки

### Стандартна збірка та запуск (Lab 01 - Lab 02):
```bash
dotnet build
dotnet run --project src/Cli
Команди публікації (Lab 02):
Bash
# Self-contained публікація (з вбудованим Runtime)
dotnet publish src/Cli -c Release -r win-x64 --self-contained true -o publish/self-contained

# Framework-dependent публікація (залежить від системного .NET 9)
dotnet publish src/Cli -c Release -r win-x64 --self-contained false -o publish/framework-dependen