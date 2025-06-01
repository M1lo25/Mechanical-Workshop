using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorOfficina.Data;
using BlazorOfficina.Data.Dtos;
using BlazorOfficina.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorOfficina.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _ctx;
        public AppointmentService(ApplicationDbContext ctx) => _ctx = ctx;

        public async Task<int> GetRegisteredVehiclesCountAsync(string userId)
            => await _ctx.Veicoli.CountAsync(v => v.UtenteId.ToString() == userId);

        public async Task<int> GetCompletedRepairsCountAsync(string userId)
            => await _ctx.Riparazioni.CountAsync(r => r.UtenteId.ToString() == userId && r.IsCompleted);

        public async Task<int> GetInProgressRepairsCountAsync(string userId)
            => await _ctx.Riparazioni.CountAsync(r => r.UtenteId.ToString() == userId && !r.IsCompleted);

        public async Task<DateTime?> GetNextAppointmentDateAsync(string userId)
            => await _ctx.Appuntamenti
                .Where(a => a.UtenteId.ToString() == userId && a.Inizio >= DateTime.Now)
                .OrderBy(a => a.Inizio)
                .Select(a => (DateTime?)a.Inizio)
                .FirstOrDefaultAsync();

        public async Task<List<RiparazioneDto>> GetOngoingRepairsAsync(string userId)
            => await _ctx.Riparazioni
                .Where(r => r.UtenteId.ToString() == userId && !r.IsCompleted)
                .Include(r => r.Utente)
                .Include(r => r.Veicolo)
                .Include(r => r.Meccanico)
                .Select(r => new RiparazioneDto(
                    r.Id,                     
                    r.Utente.Username,      
                    r.Veicolo.Targa,       
                    r.Veicolo.Modello,        
                    r.DataProntoRitiro,       
                    r.IsCompleted,           
                    r.Costo,                  
                    r.MeccanicoId,          
                    r.Meccanico.Username      
                ))
                .ToListAsync();

        public async Task<List<AppuntamentoDto>> GetUpcomingAppointmentsAsync(string userId, int count)
            => await _ctx.Appuntamenti
                .Where(a => a.UtenteId.ToString() == userId && a.Inizio >= DateTime.Now)
                .OrderBy(a => a.Inizio)
                .Take(count)
                .Select(a => new AppuntamentoDto { Id = a.Id, Tipo = a.Servizio.Nome, ModelloVeicolo = a.Veicolo.Modello, TargaVeicolo = a.Veicolo.Targa, DataAppuntamento = a.Inizio })
                .ToListAsync();

        public async Task<List<PreventivoDto>> GetRecentEstimatesAsync(string userId, int count)
            => await _ctx.Preventivi
                .Where(p => p.UtenteId.ToString() == userId)
                .OrderByDescending(p => p.DataCreazione)
                .Take(count)
                .Select(p => new PreventivoDto { Id = p.Id, Titolo = p.Titolo, ModelloVeicolo = p.Veicolo.Modello, TargaVeicolo = p.Veicolo.Targa, Totale = p.Totale })
                .ToListAsync();

        public async Task CancelAppointmentAsync(int appointmentId)
        {
            var appt = new Appuntamento { Id = appointmentId };
            _ctx.Appuntamenti.Attach(appt).State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
        }

        public async Task<List<AppuntamentoDto>> GetAppointmentsForDateAsync(DateTime date)
        {
            return await _ctx.Appuntamenti
                .Where(a => a.Inizio.Date == date.Date)
                .Select(a => new AppuntamentoDto
                {
                    Id = a.Id,
                    Tipo = a.Servizio.Nome,
                    ModelloVeicolo = a.Veicolo.Modello,
                    TargaVeicolo = a.Veicolo.Targa,
                    DataAppuntamento = a.Inizio
                })
                .ToListAsync();
        }

        public async Task<int> GetTodaysCountForMechanicAsync(string mechanicId)
        {
            var today = DateTime.UtcNow.Date;
            int mid = int.Parse(mechanicId);

            return await _ctx.Appuntamenti
                             .CountAsync(a =>
                                 a.MeccanicoId == mid &&
                                 a.Inizio.Date == today);
        }

        public async Task<IEnumerable<MechanicAppointmentDto>> GetByMechanicAsync(string mechanicId)
        {
            if (!int.TryParse(mechanicId, out var mid))
                return Enumerable.Empty<MechanicAppointmentDto>();

            var now = DateTime.UtcNow;
            var list = await _ctx.Appuntamenti
                .Include(a => a.Servizio)
                .Include(a => a.Veicolo)
                .Include(a => a.Utente)
                .Where(a => a.MeccanicoId == mid)
                .OrderBy(a => a.Inizio)
                .ToListAsync();

            return list.Select(a => {
                var stato = a.Inizio > now
                    ? AppointmentStatus.InAttesa
                    : (a.Fine < now
                       ? AppointmentStatus.Completato
                       : AppointmentStatus.InLavorazione);

                return new MechanicAppointmentDto
                {
                    Id = a.Id,
                    ClienteId = a.UtenteId,
                    ClienteNome = a.Utente.Username,
                    VeicoloId = a.VeicoloId,
                    VeicoloTarga = a.Veicolo.Targa,
                    ServizioNome = a.Servizio.Nome,
                    Inizio = a.Inizio,
                    Fine = a.Fine,
                    CostoStimato = a.CostoStimato,
                    Stato = stato
                };
            });
        }

        public async Task CancelAppointmentAsync(int appointmentId, string userId)
        {
            var appt = await _ctx.Appuntamenti
                .FirstOrDefaultAsync(a =>
                   a.Id == appointmentId &&
                   a.Veicolo.UtenteId.ToString() == userId);

            if (appt is null)
                throw new InvalidOperationException("Appuntamento non trovato o non autorizzato.");

            _ctx.Appuntamenti.Remove(appt);
            await _ctx.SaveChangesAsync();
        }

        public async Task<int> GetCurrentAppointmentsCountForMechanicAsync(string mechanicId)
        {
            if (!int.TryParse(mechanicId, out var mid))
                return 0;

            var now = DateTime.UtcNow;
            return await _ctx.Appuntamenti
                .Where(a =>
                    a.MeccanicoId == mid
                    && a.Inizio <= now && now < a.Fine
                )
                .CountAsync();
        }

        public async Task<int> GetCurrentAppointmentsCountForCustomerAsync(string userId)
        {
            var now = DateTime.UtcNow;
            return await _ctx.Appuntamenti
                .Where(a => a.UtenteId.ToString() == userId
                         && a.Inizio <= now
                         && now < a.Fine)
                .CountAsync();
        }
  
        public async Task<int> GetCompletedAppointmentsCountForCustomerAsync(string userId)
        {
            var now = DateTime.UtcNow;
            return await _ctx.Appuntamenti
                .Where(a => a.UtenteId.ToString() == userId
                         && a.Fine < now)   
                .CountAsync();
        }


    }
}