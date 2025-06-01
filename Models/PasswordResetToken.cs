using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorOfficina.Data.Models
{
    public class PasswordResetToken
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public DateTime Expiry { get; set; }
    }
}
