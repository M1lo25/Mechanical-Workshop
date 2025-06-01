using System;

namespace BlazorOfficina.Data.Dtos
{
    /// <summary>
    /// Modalità di preferenza di contatto.
    /// </summary>
    [Flags]
    public enum ContactPreferences
    {
        None = 0,
        Email = 1 << 0,
        Sms = 1 << 1,
        WhatsApp = 1 << 2
    }

    /// <summary>
    /// Modalità di consegna/ritiro del veicolo.
    /// </summary>
    public enum DeliveryMode
    {
        InOfficina,
        RitiroADomicilio
    }

    /// <summary>
    /// DTO per la creazione di un nuovo appuntamento.
    /// </summary>
    public record CreateAppointmentDto(
        int VehicleId,
        int ServiceId,
        DateTime Start,
        DateTime End,
        string Notes,
        DeliveryMode DeliveryMode,
        ContactPreferences ContactPreferences
    );
}
