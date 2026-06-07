using PersonalExpensesManager.Shared.Dto;

namespace PersonalExpensesManager.Api.Models;

public class Transaction
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public TransactionType Type { get; set; } = TransactionType.Expense;
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public int AccountId { get; set; }
    public Account? Account { get; set; }
    public string? Note { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
}
