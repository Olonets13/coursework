namespace PersonalExpensesManager.Shared.Dto;

public class MonthlySummaryDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Income { get; set; }
    public decimal Expenses { get; set; }
    public decimal Balance => Income - Expenses;
}
