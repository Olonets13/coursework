using System.ComponentModel.DataAnnotations;

namespace PersonalExpensesManager.Shared.Dto;

public class RegisterDto
{
    [Required, StringLength(80)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(120)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6), StringLength(80)]
    public string Password { get; set; } = string.Empty;
}
