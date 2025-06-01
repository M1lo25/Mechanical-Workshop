// File: Services/IAppointmentBookingService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorOfficina.Data.Dtos;
using BlazorOfficina.Data.Models;

namespace BlazorOfficina.Services
{
    /// <summary>
    /// Logica di dominio per prenotare appuntamenti:
    /// - elenco servizi
    /// - slot disponibili
    /// - calcolo preventivo
    /// - creazione appuntamento
    /// - recupero dettaglio appuntamento
    /// </summary>
    public interface IBookingService
    {
        /// <summary>
        /// Restituisce tutti i servizi disponibili.
        /// </summary>
        Task<List<ServiceDto>> GetAllServicesAsync();

        /// <summary>
        /// Restituisce gli slot orari liberi per un servizio, una data e un veicolo di proprietà dell'utente.
        /// </summary>
        Task<List<AvailableSlotDto>> GetAvailableSlotsAsync(
            string userId,
            int serviceId,
            DateTime date,
            int vehicleId
        );

        /// <summary>
        /// Calcola il preventivo per un servizio e un veicolo specifico di proprietà dell'utente.
        /// </summary>
        

        /// <summary>
        /// Crea un nuovo appuntamento nel sistema per l'utente.
        /// </summary>
        Task<Appuntamento> CreateAsync(
            string userId,
            CreateAppointmentDto dto
        );

        /// <summary>
        /// Recupera i dettagli di un appuntamento per ID e utente.
        /// </summary>
        Task<Appuntamento?> GetByIdAsync(
            string userId,
            int appointmentId
        );

        Task<List<AppuntamentoDto>> GetRecentAppointmentsAsync(string userId);

        Task<List<DateTime>> GetAvailableDatesAsync(
            string userId,
            int serviceId,
            int vehicleId,
            DateTime month
        );

        Task<List<AppuntamentoDto>> GetAppointmentsByDateAsync(DateTime date);
    }
}
