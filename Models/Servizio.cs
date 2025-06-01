using System.ComponentModel.DataAnnotations;

namespace BlazorOfficina.Data.Models
{
    public class Servizio
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal PrezzoBase { get; set; }

        [Required]
        public int DurataMinuti { get; set; }

        [Required]
        public int DurataMassimi { get; set; }

        // ← aggiungi questa
        [Required]
        public string Descrizione { get; set; } = string.Empty;
    }

}
