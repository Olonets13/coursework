namespace PersonalExpensesManager.Api.Models;

public class Budget
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public decimal Limit { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
}
