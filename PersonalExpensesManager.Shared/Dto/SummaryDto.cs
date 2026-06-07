namespace PersonalExpensesManager.Shared.Dto;

public class SummaryDto
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal Balance { get; set; }
    public decimal MonthIncome { get; set; }
    public decimal MonthExpenses { get; set; }
    public decimal MonthBalance => MonthIncome - MonthExpenses;
    public int TransactionsCount { get; set; }
}
