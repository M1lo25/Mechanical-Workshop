using System.ComponentModel.DataAnnotations;

public class LoginDto
{
    [Required] public string Identifier { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
    // aggiungi questa:
    public string? ReturnUrl { get; set; }
}
