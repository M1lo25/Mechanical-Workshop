using BlazorOfficina.Data.Models;
using System.ComponentModel.DataAnnotations;


namespace BlazorOfficina.Data.Dtos
{
    public class AggiungiVeicoloDto
    {
        [Required]
        public CategoriaVeicolo Categoria { get; set; }   // Utilitaria, Berlina…

        [Required]
        public string Marca { get; set; } = string.Empty;

        [Required]
        public string Modello { get; set; } = string.Empty;

        [Required]
        public int Anno { get; set; }

        [Required]
        [StringLength(10)]
        public string Targa { get; set; } = string.Empty;

        [Required]
        public int Chilometraggio { get; set; }

        [Required]
        public string Carburante { get; set; } = string.Empty;

        public string? Note { get; set; }

        public bool LibrettoCircolazione { get; set; }

        public bool ManualeUso { get; set; }

        public bool ChiaviDiRiserva { get; set; }
    }
}
