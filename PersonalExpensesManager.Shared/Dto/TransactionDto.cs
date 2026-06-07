using System.ComponentModel.DataAnnotations;

namespace PersonalExpensesManager.Shared.Dto;

public class TransactionDto
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Title { get; set; } = string.Empty;

    [Range(0.01, 999999999)]
    public decimal Amount { get; set; }

    public DateTime Date { get; set; } = DateTime.Today;
    public TransactionType Type { get; set; } = TransactionType.Expense;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryColor { get; set; } = "#3B82F6";
    public int AccountId { get; set; }
    public string AccountName { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Note { get; set; }
}
