namespace PersonalExpensesManager.Shared.Dto;

public class CategorySummaryDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryColor { get; set; } = "#3B82F6";
    public decimal Amount { get; set; }
    public int Count { get; set; }
}
