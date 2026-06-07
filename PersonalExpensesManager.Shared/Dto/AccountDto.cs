using System.ComponentModel.DataAnnotations;

namespace PersonalExpensesManager.Shared.Dto;

public class AccountDto
{
    public int Id { get; set; }

    [Required, StringLength(80)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(10)]
    public string Currency { get; set; } = "UAH";

    public decimal InitialBalance { get; set; }
    public decimal CurrentBalance { get; set; }
}
