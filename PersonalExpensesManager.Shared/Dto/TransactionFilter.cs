namespace PersonalExpensesManager.Shared.Dto;

public class TransactionFilter
{
    public TransactionType? Type { get; set; }
    public int? CategoryId { get; set; }
    public int? AccountId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public string? Search { get; set; }
}
