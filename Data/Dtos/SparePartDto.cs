namespace BlazorOfficina.Data.Dtos
{
    /// <summary>
    /// DTO per rappresentare un ricambio disponibile.
    /// </summary>
    public record SparePartDto(
        int Id,
        string Name,
        string Code,
        decimal Price
    );
}
