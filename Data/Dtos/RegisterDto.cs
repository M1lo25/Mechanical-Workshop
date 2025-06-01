using System.ComponentModel.DataAnnotations;

public class RegisterDto
{
    [Required(ErrorMessage = "Username obbligatorio")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email obbligatoria")]
    [RegularExpression(
        @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.(com|it|de|net|org|info|biz|co|io|us|uk|de|fr|es|ca|au|nl|ru|jp|br|eu|pl|inedu|gov|mil|int)$",
        ErrorMessage = "Formato email non valido (deve terminare con es. .com)"
    )]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password obbligatoria")]
    [RegularExpression(
        @"^(?=.*[A-Z])(?=.*\d)(?=.*\W).{8,}$",
        ErrorMessage = "La password deve contenere almeno 8 caratteri, una lettera maiuscola, un numero e un carattere speciale"
    )]
    public string Password { get; set; } = string.Empty;

    public string Role { get; set; } = "Customer";
}
