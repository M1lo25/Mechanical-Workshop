using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorOfficina.Data.Dtos
{
    public record RiparazioneDto(
    int Id,
    string ModelloVeicolo,
    string CustomerName,
    string VehicleTarga,
    DateTime DataProntoRitiro,
    bool IsCompleted,
    decimal Costo,
    int MeccanicoId,
    string MeccanicoName
);

}