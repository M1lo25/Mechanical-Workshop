using System;
using System.ComponentModel.DataAnnotations;
using BlazorOfficina.Data.Models;

namespace BlazorOfficina.Data.Dtos
{
    public class VeicoloDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Seleziona la categoria del veicolo")]
        public CategoriaVeicolo Categoria { get; set; }

        [Required(ErrorMessage = "Marca obbligatoria")]
        public string Marca { get; set; } = string.Empty;

        [Required(ErrorMessage = "Modello obbligatorio")]
        public string Modello { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleziona l'anno")]
        [Range(1950, 2100, ErrorMessage = "Anno non valido")]
        public int? Anno { get; set; }

        [Required(ErrorMessage = "Targa obbligatoria")]
        [StringLength(10, ErrorMessage = "Massimo 10 caratteri per la targa")]
        public string Targa { get; set; } = string.Empty;

        [Required(ErrorMessage = "Chilometraggio obbligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "Valore non valido")]
        public int? Chilometraggio { get; set; }

        [Required(ErrorMessage = "Seleziona tipo di carburante")]
        public string Carburante { get; set; } = string.Empty;

        public string? Note { get; set; }
        public bool LibrettoCircolazione { get; set; }
        public bool ManualeUso { get; set; }
        public bool ChiaviDiRiserva { get; set; }

        public int UtenteId { get; set; }

        // ——————————————————————————————————
        // Campi per la dashboard “Vehicles”
        // ——————————————————————————————————

        /// <summary>
        /// Stato corrente del veicolo (InAttesa, InRiparazione, Completato).
        /// </summary>
        public StatoRiparazione Stato { get; set; }

        /// <summary>
        /// Percentuale di avanzamento (0–100).
        /// </summary>
        public int PercentualeRiparazione { get; set; }

        /// <summary>
        /// Descrizione breve dell’intervento in corso o del prossimo intervento.
        /// </summary>
        public string InterventoCorrente { get; set; } = string.Empty;

        /// <summary>
        /// Data di inizio lavori (se applicabile).
        /// </summary>
        public DateTime? DataInizio { get; set; }

        /// <summary>
        /// Data stimata di completamento o data del prossimo intervento.
        /// </summary>
        public DateTime? DataPrevisto { get; set; }

        /// <summary>
        /// Data dell’ultimo intervento (per la sezione “Tutte le auto”).
        /// </summary>
        public DateTime? UltimoIntervento { get; set; }

        /// <summary>
        /// Spesa cumulata di tutti gli interventi.
        /// </summary>
        public decimal SpesaTotale { get; set; }
    }
}
