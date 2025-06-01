using BlazorOfficina.Data.Dtos;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorOfficina.Data.Models
{
    public enum StatoAppuntamento
    {
        InLavorazione,
        InParcheggio,
        DaRevisionare,
        InAttesa
    }

    public class Appuntamento
    {
        public int Id { get; set; }

        // Rimosso [Required] perché MeccanicoId deve poter restare null finché l’admin 
        // non lo assegna esplicitamente
        public int? MeccanicoId { get; set; }
        public User? Meccanico { get; set; }

        [Required]
        public int UtenteId { get; set; }
        public User Utente { get; set; } = null!;

        [Required]
        public int VeicoloId { get; set; }

        [ForeignKey(nameof(VeicoloId))]
        [InverseProperty(nameof(Veicolo.Appuntamenti))]
        public Veicolo Veicolo { get; set; } = null!;

        [Required]
        public int ServizioId { get; set; }
        public Servizio Servizio { get; set; } = null!;

        [Required]
        public DateTime Inizio { get; set; }

        [Required]
        public DateTime Fine { get; set; }

        public string? Note { get; set; }

        [Required]
        public DeliveryMode ModalitaConsegna { get; set; }

        [Required]
        public ContactPreferences PreferenzeContatto { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal CostoStimato { get; set; }

        [Required]
        public StatoAppuntamento Stato { get; set; }
    }
}
