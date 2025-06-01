using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorOfficina.Data.Dtos
{
    public class AppuntamentoDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Tipo { get; set; } = null!;

        [Required]
        public string ModelloVeicolo { get; set; } = null!;

        [Required]
        public string TargaVeicolo { get; set; } = null!;

        [Required]
        public DateTime DataAppuntamento { get; set; }

        public DateTime EndTime { get; set; }
    }
}