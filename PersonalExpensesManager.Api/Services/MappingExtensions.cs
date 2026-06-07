using PersonalExpensesManager.Api.Models;
using PersonalExpensesManager.Shared.Dto;

namespace PersonalExpensesManager.Api.Services;

public static class MappingExtensions
{
    public static CategoryDto ToDto(this Category category) => new()
    {
        Id = category.Id,
        Name = category.Name,
        Color = category.Color,
        Kind = category.Kind
    };

    public static AccountDto ToDto(this Account account, decimal currentBalance) => new()
    {
        Id = account.Id,
        Name = account.Name,
        Currency = account.Currency,
        InitialBalance = account.InitialBalance,
        CurrentBalance = currentBalance
    };

    public static TransactionDto ToDto(this Transaction transaction) => new()
    {
        Id = transaction.Id,
        Title = transaction.Title,
        Amount = transaction.Amount,
        Date = transaction.Date,
        Type = transaction.Type,
        CategoryId = transaction.CategoryId,
        CategoryName = transaction.Category?.Name ?? string.Empty,
        CategoryColor = transaction.Category?.Color ?? "#3B82F6",
        AccountId = transaction.AccountId,
        AccountName = transaction.Account?.Name ?? string.Empty,
        Note = transaction.Note
    };
}
