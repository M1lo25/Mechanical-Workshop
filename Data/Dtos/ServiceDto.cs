using System;

namespace BlazorOfficina.Data.Dtos
{
    /// <summary>
    /// DTO per rappresentare un servizio disponibile.
    /// </summary>
    public record ServiceDto(
        int Id,
        string Name,
        decimal BasePrice,
        string Description,
        string DurationRange
    );
}