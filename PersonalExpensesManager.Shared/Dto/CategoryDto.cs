using System.ComponentModel.DataAnnotations;

namespace PersonalExpensesManager.Shared.Dto;

public class CategoryDto
{
    public int Id { get; set; }

    [Required, StringLength(80)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(20)]
    public string Color { get; set; } = "#3B82F6";

    public CategoryKind Kind { get; set; } = CategoryKind.Expense;
}
