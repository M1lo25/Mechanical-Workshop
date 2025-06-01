using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorOfficina.Data.Models
{
    public class Riparazione
    {
        public int Id { get; set; }

        [Required]
        public int UtenteId { get; set; }
        public User Utente { get; set; } = null!;

        [Required]
        public int MeccanicoId { get; set; }
        public User Meccanico { get; set; } = null!;

        [Required]
        public int VeicoloId { get; set; }
        public Veicolo Veicolo { get; set; } = null!;
        public bool IsCompleted { get; set; }

        [Required]
        public DateTime DataProntoRitiro { get; set; }

        [Required]
        public string Descrizione { get; set; } = string.Empty;

        [Required]
        public int Chilometraggio { get; set; }

        [Required]
        public decimal Costo { get; set; }
    }
}
