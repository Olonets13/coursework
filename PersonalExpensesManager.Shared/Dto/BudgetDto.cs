using System.ComponentModel.DataAnnotations;

namespace PersonalExpensesManager.Shared.Dto;

public class BudgetDto
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryColor { get; set; } = "#3B82F6";

    [Range(0.01, 999999999)]
    public decimal Limit { get; set; }

    public int Month { get; set; } = DateTime.Today.Month;
    public int Year { get; set; } = DateTime.Today.Year;
    public decimal Spent { get; set; }
    public decimal Remaining => Limit - Spent;
    public decimal UsedPercent => Limit == 0 ? 0 : Math.Min(100, Math.Round(Spent / Limit * 100, 1));
}
