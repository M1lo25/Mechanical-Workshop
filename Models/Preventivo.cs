using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorOfficina.Data.Models
{

    public enum PreventivoStatus
    {
        Pending,    
        Confirmed, 
        Rejected,   
        Modified    
    }

    public class Preventivo
    {
        public int Id { get; set; }

        [Required]
        public int UtenteId { get; set; }
        public User Utente { get; set; } = null!;

        [Required]
        public int VeicoloId { get; set; }
        public Veicolo Veicolo { get; set; } = null!;

        // Questo diventa il campo principale di descrizione
        [Required, StringLength(500)]
        public string Titolo { get; set; } = string.Empty;

        [Required]
        public int ServizioId { get; set; }
        public Servizio Servizio { get; set; } = null!;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Totale { get; set; }

        [Required]
        public DateTime DataCreazione { get; set; }

        public bool IsConfirmed { get; set; } = false;

        public int? MeccanicoId { get; set; }      // ora può essere null
        public User? Meccanico { get; set; }

        public PreventivoStatus Status { get; set; } = PreventivoStatus.Pending;
    }
}
