using System.ComponentModel.DataAnnotations;

namespace BlazorOfficina.Data.Models
{
    public class Ricambio
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Codice { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Prezzo { get; set; }
    }
}
