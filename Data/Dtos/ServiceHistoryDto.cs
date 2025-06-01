// Data/Dtos/ServiceHistoryDto.cs
using System;

namespace BlazorOfficina.Data.Dtos
{
    public class ServiceHistoryDto
    {
        public string Descrizione { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public int Kilometri { get; set; }
        public decimal Costo { get; set; }
    }
}
