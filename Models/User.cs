using System.ComponentModel.DataAnnotations;

namespace BlazorOfficina.Data.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;

        // <<< nuove proprietà >>>
        [Required]
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        public byte[]? ProfileImage { get; set; }
    }

}
