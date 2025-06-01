using System.ComponentModel.DataAnnotations;

namespace BlazorOfficina.Data.Dtos
{
    public class CreatePreventivoDto
    {
        [Required]
        public int VeicoloId { get; set; }

        [Required]
        public int ServizioId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Il totale stimato deve essere ≥ 0")]
        public decimal? TotaleStimato { get; set; }

        // Nuova proprietà per la descrizione del preventivo
        [StringLength(500, ErrorMessage = "La descrizione può contenere al massimo 500 caratteri")]
        public string Descrizione { get; set; } = string.Empty;

        public List<int> ServiziIds { get; set; } = new();

        public List<SparePartItemDto> Ricambi { get; set; } = new();
    }
}
