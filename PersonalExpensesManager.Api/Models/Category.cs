using PersonalExpensesManager.Shared.Dto;

namespace PersonalExpensesManager.Api.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#3B82F6";
    public CategoryKind Kind { get; set; } = CategoryKind.Expense;
    public int UserId { get; set; }
    public User? User { get; set; }
    public List<Transaction> Transactions { get; set; } = [];
    public List<Budget> Budgets { get; set; } = [];
}
