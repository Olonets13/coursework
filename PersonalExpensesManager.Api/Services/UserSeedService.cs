using PersonalExpensesManager.Api.Models;
using PersonalExpensesManager.Shared.Dto;

namespace PersonalExpensesManager.Api.Services;

public class UserSeedService
{
    public static void AddStarterData(User user, string currency = "UAH")
    {
        var groceries = new Category { Name = "Продукти", Color = "#2F855A", Kind = CategoryKind.Expense, User = user };
        var transport = new Category { Name = "Транспорт", Color = "#DD6B20", Kind = CategoryKind.Expense, User = user };
        var cafe = new Category { Name = "Кафе", Color = "#C53030", Kind = CategoryKind.Expense, User = user };
        var utilities = new Category { Name = "Комунальні", Color = "#5A67D8", Kind = CategoryKind.Expense, User = user };
        var salary = new Category { Name = "Зарплата", Color = "#2B6CB0", Kind = CategoryKind.Income, User = user };
        var gifts = new Category { Name = "Подарунки", Color = "#B83280", Kind = CategoryKind.Universal, User = user };

        user.Categories.AddRange([groceries, transport, cafe, utilities, salary, gifts]);

        var cash = new Account { Name = "Готівка", Currency = currency, InitialBalance = 2500, User = user };
        var card = new Account { Name = "Банківська карта", Currency = currency, InitialBalance = 12000, User = user };

        user.Accounts.AddRange([cash, card]);

        user.Transactions.AddRange([
            new Transaction { Title = "Зарплата за місяць", Amount = 28000, Date = new DateTime(2026, 6, 1), Type = TransactionType.Income, Category = salary, Account = card, User = user },
            new Transaction { Title = "Супермаркет", Amount = 1350, Date = new DateTime(2026, 6, 1), Type = TransactionType.Expense, Category = groceries, Account = card, User = user },
            new Transaction { Title = "Метро і таксі", Amount = 420, Date = new DateTime(2026, 6, 2), Type = TransactionType.Expense, Category = transport, Account = cash, User = user },
            new Transaction { Title = "Кава з друзями", Amount = 310, Date = new DateTime(2026, 6, 2), Type = TransactionType.Expense, Category = cafe, Account = card, User = user }
        ]);

        user.Budgets.AddRange([
            new Budget { Category = groceries, Limit = 8000, Month = 6, Year = 2026, User = user },
            new Budget { Category = transport, Limit = 2500, Month = 6, Year = 2026, User = user },
            new Budget { Category = cafe, Limit = 3000, Month = 6, Year = 2026, User = user }
        ]);
    }
}
