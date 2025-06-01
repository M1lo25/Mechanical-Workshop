namespace BlazorOfficina.Data.Dtos
{
    /// <summary>
    /// Rappresenta un ricambio selezionato con quantità.
    /// </summary>
    public record SparePartItemDto(
        int SparePartId,
        int Quantity
    );
}
