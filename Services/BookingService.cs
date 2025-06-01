using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BlazorOfficina.Data;
using BlazorOfficina.Data.Dtos;
using BlazorOfficina.Data.Models;

namespace BlazorOfficina.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _ctx;

        public BookingService(ApplicationDbContext ctx)
            => _ctx = ctx;

        public async Task<List<ServiceDto>> GetAllServicesAsync()
            => await _ctx.Servizi
                .Select(s => new ServiceDto(
                    s.Id,
                    s.Nome,
                    s.PrezzoBase,
                    s.Descrizione,
                    $"{s.DurataMinuti}-{s.DurataMassimi} min"))
                .ToListAsync();

        public async Task<List<AvailableSlotDto>> GetAvailableSlotsAsync(
            string userId,
            int serviceId,
            DateTime date,
            int vehicleId)
        {
            // Verifica che il veicolo appartenga all'utente
            var veicolo = await _ctx.Veicoli
                .FirstOrDefaultAsync(v => v.Id == vehicleId && v.UtenteId.ToString() == userId);
            if (veicolo is null)
                throw new UnauthorizedAccessException("Veicolo non valido o non di proprietà dell'utente.");

            var apertura = date.Date.AddHours(9);
            var chiusura = date.Date.AddHours(18);

            var servizio = await _ctx.Servizi.FindAsync(serviceId)
                              ?? throw new InvalidOperationException("Servizio non trovato.");
            var minDuration = TimeSpan.FromMinutes(servizio.DurataMinuti);

            var prenotati = await _ctx.Appuntamenti
                .Where(a => a.Inizio.Date == date.Date)
                .Select(a => new { a.Inizio, a.Fine })
                .ToListAsync();

            var slots = new List<AvailableSlotDto>();
            var cursor = apertura;
            while (cursor + minDuration <= chiusura)
            {
                var end = cursor + minDuration;
                bool conflitto = prenotati.Any(r => cursor < r.Fine && end > r.Inizio);
                if (!conflitto)
                    slots.Add(new AvailableSlotDto(cursor, end));
                cursor = cursor.AddMinutes(15);
            }
            return slots;
        }

        public async Task<List<AppuntamentoDto>> GetRecentAppointmentsAsync(string userId)
        {
            return await _ctx.Appuntamenti
                .Include(a => a.Servizio)
                .Where(a => a.Veicolo.UtenteId.ToString() == userId)
                .OrderBy(a => a.Inizio)               // ordina cronologicamente
                .Select(a => new AppuntamentoDto
                {
                    Id = a.Id,
                    Tipo = a.Servizio.Nome,
                    DataAppuntamento = a.Inizio
                })
                .ToListAsync();
        }

        public async Task<Appuntamento> CreateAsync(
    string userId,
    CreateAppointmentDto dto)
        {
            if (!int.TryParse(userId, out var uid))
                throw new UnauthorizedAccessException("ID utente non valido.");

            var veicolo = await _ctx.Veicoli
                .FirstOrDefaultAsync(v =>
                    v.Id == dto.VehicleId &&
                    v.UtenteId == uid);
            if (veicolo is null)
                throw new UnauthorizedAccessException("Veicolo non valido o non di proprietà.");

            var servizio = await _ctx.Servizi
                .FirstOrDefaultAsync(s => s.Id == dto.ServiceId);
            if (servizio is null)
                throw new InvalidOperationException("Servizio selezionato non esiste.");

            var appt = new Appuntamento
            {
                VeicoloId = dto.VehicleId,
                ServizioId = dto.ServiceId,
                Inizio = dto.Start,
                Fine = dto.End,
                Note = dto.Notes,
                ModalitaConsegna = dto.DeliveryMode,
                PreferenzeContatto = dto.ContactPreferences,
                UtenteId = uid,
                MeccanicoId = null,                    
                CostoStimato = servizio.PrezzoBase,    
                Stato = StatoAppuntamento.InAttesa    
            };

            _ctx.Appuntamenti.Add(appt);
            await _ctx.SaveChangesAsync();
            return appt;
        }

        public async Task<Appuntamento?> GetByIdAsync(
            string userId,
            int appointmentId)
        {
            return await _ctx.Appuntamenti
                .Include(a => a.Servizio)
                .Include(a => a.Veicolo)
                .FirstOrDefaultAsync(a =>
                    a.Id == appointmentId &&
                    a.Veicolo.UtenteId.ToString() == userId);
        }

        public async Task<List<DateTime>> GetAvailableDatesAsync(
    string userId,
    int serviceId,
    int vehicleId,
    DateTime month)
        {
            var dates = new List<DateTime>();
            // inizio e fine del mese
            var start = new DateTime(month.Year, month.Month, 1);
            var end = start.AddMonths(1);

            // per ogni giorno del mese, verifica se esistono slot liberi
            for (var day = start; day < end; day = day.AddDays(1))
            {
                var slots = await GetAvailableSlotsAsync(
                    userId,
                    serviceId,
                    day,
                    vehicleId
                );
                if (slots.Any())
                {
                    dates.Add(day.Date);
                }
            }

            return dates;
        }

        public async Task<List<AppuntamentoDto>> GetAppointmentsByDateAsync(DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = date.Date.AddDays(1);

            var list = await _ctx.Appuntamenti
                .Where(a => a.Inizio >= startOfDay && a.Inizio < endOfDay)
                .Select(a => new AppuntamentoDto
                {
                    Id = a.Id,
                    DataAppuntamento = a.Inizio,
                    EndTime = a.Fine,
                    Tipo = a.Servizio.Nome 
                })
                .ToListAsync();

            return list;
        }

    }

}
