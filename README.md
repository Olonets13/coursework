# Менеджер особистих витрат

Курсовий C# full-stack проект для обліку особистих доходів, витрат, рахунків, категорій, бюджетів і статистики.

## Технології

- ASP.NET Core Web API
- Blazor WebAssembly
- Entity Framework Core
- SQLite
- JWT авторизація

## Демо користувач

```text
Email: demo@example.com
Password: password123
```

Після входу користувач бачить тільки власні рахунки, категорії, операції, бюджети і статистику.

## Структура

- `PersonalExpensesManager.Api` - backend, база даних, REST API.
- `PersonalExpensesManager.Client` - frontend на Blazor WebAssembly.
- `PersonalExpensesManager.Shared` - спільні DTO та enum-и.

## Запуск

У першому терміналі:

```bash
dotnet run --project PersonalExpensesManager.Api/PersonalExpensesManager.Api.csproj --launch-profile http
```

У другому терміналі:

```bash
dotnet run --project PersonalExpensesManager.Client/PersonalExpensesManager.Client.csproj --launch-profile http
```

Після запуску:

- Frontend: `http://localhost:5259`
- API Swagger: `http://localhost:5012/swagger`

SQLite база створюється автоматично при першому запуску API.
