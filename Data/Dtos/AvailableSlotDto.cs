using System;

namespace BlazorOfficina.Data.Dtos
{
    /// <summary>
    /// DTO per rappresentare uno slot orario disponibile.
    /// </summary>
    public record AvailableSlotDto(
        DateTime Start,
        DateTime End
    );
}