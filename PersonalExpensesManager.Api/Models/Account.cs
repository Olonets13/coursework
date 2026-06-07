namespace PersonalExpensesManager.Api.Models;

public class Account
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Currency { get; set; } = "UAH";
    public decimal InitialBalance { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public List<Transaction> Transactions { get; set; } = [];
}
