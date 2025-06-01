using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BlazorOfficina.Data.Models;

namespace BlazorOfficina.Data.Models
{
    public enum CategoriaVeicolo
    {
        [Display(Name = "Auto")]
        Auto,

        [Display(Name = "Pickup")]
        Pickup,

        [Display(Name = "Furgone")]
        Furgone,

        [Display(Name = "Moto")]
        Moto
    }

    public enum StatoRiparazione
    {
        InAttesa,
        InRiparazione,
        Completato
    }

    public class Veicolo
    {
        public int Id { get; set; }

        [Required]
        public CategoriaVeicolo Categoria { get; set; }

        [Required, StringLength(50)]
        public string Marca { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Modello { get; set; } = string.Empty;

        [Required, Range(1900, 2100)]
        public int Anno { get; set; }

        [Required, StringLength(10)]
        public string Targa { get; set; } = string.Empty;

        [Required, Range(0, int.MaxValue)]
        public int Chilometraggio { get; set; }

        [Required, StringLength(20)]
        public string Carburante { get; set; } = string.Empty;

        public string? Note { get; set; }

        public bool LibrettoCircolazione { get; set; }
        public bool ManualeUso { get; set; }
        public bool ChiaviDiRiserva { get; set; }

        [Required]
        public int UtenteId { get; set; }
        public User Utente { get; set; } = null!;

        // Stato officina
        [Required]
        public StatoRiparazione Stato { get; set; } = StatoRiparazione.InAttesa;

        [Range(0, 100)]
        public int PercentualeRiparazione { get; set; }

        [StringLength(200)]
        public string InterventoCorrente { get; set; } = string.Empty;

        public DateTime? DataInizio { get; set; }
        public DateTime? DataPrevisto { get; set; }

        // NAVIGATION PROPERTIES

        /// <summary>
        /// Tutte le riparazioni storiche o in corso.
        /// </summary>
        public List<Riparazione> Riparazioni { get; set; } = new();

        /// <summary>
        /// Tutti gli appuntamenti associati a questo veicolo.
        /// </summary>
        public List<Appuntamento> Appuntamenti { get; set; } = new();
    }
}
