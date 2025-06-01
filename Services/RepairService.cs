// Services/RepairService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BlazorOfficina.Data;
using BlazorOfficina.Data.Dtos;

namespace BlazorOfficina.Services
{
    public class RepairService : IRepairService
    {
        private readonly ApplicationDbContext _ctx;

        public RepairService(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        /// <summary>
        /// Restituisce tutte le riparazioni ancora in corso.
        /// </summary>
        public async Task<List<RiparazioneDto>> GetOngoingRepairsAsync()
        {
            return await _ctx.Riparazioni
                .Where(r => !r.IsCompleted)
                .Include(r => r.Utente)
                .Include(r => r.Veicolo)
                .Include(r => r.Meccanico)
                .Select(r => new RiparazioneDto(
                    r.Id,
                    r.Utente.Username,
                    r.Veicolo.Targa,
                    r.Veicolo.Modello,       // ← ModelloVeicolo
                    r.DataProntoRitiro,
                    r.IsCompleted,
                    r.Costo,
                    r.MeccanicoId,
                    r.Meccanico.Username
                ))
                .ToListAsync();
        }

        /// <summary>
        /// Restituisce i dettagli di una singola riparazione.
        /// </summary>
        public async Task<RiparazioneDto> GetRepairByIdAsync(int id)
        {
            var r = await _ctx.Riparazioni
                .Include(r => r.Utente)
                .Include(r => r.Veicolo)
                .Include(r => r.Meccanico)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (r is null)
                throw new KeyNotFoundException($"Riparazione con ID {id} non trovata.");

            return new RiparazioneDto(
                r.Id,
                r.Utente.Username,
                r.Veicolo.Targa,
                r.Veicolo.Modello,       // ← ModelloVeicolo
                r.DataProntoRitiro,
                r.IsCompleted,
                r.Costo,
                r.MeccanicoId,
                r.Meccanico.Username
            );
        }

        /// <summary>
        /// Restituisce tutte le riparazioni assegnate al meccanico loggato.
        /// </summary>
        public async Task<IEnumerable<RiparazioneDto>> GetByMechanicAsync(string mechanicId)
        {
            if (!int.TryParse(mechanicId, out var mechInt))
                return Enumerable.Empty<RiparazioneDto>();

            return await _ctx.Riparazioni
                .Where(r => r.MeccanicoId == mechInt)
                .Include(r => r.Utente)
                .Include(r => r.Veicolo)
                .Include(r => r.Meccanico)
                .Select(r => new RiparazioneDto(
                    r.Id,
                    r.Utente.Username,
                    r.Veicolo.Targa,
                    r.Veicolo.Modello,       // ← ModelloVeicolo
                    r.DataProntoRitiro,
                    r.IsCompleted,
                    r.Costo,
                    r.MeccanicoId,
                    r.Meccanico.Username
                ))
                .ToListAsync();
        }

        public async Task<int> GetInProgressCountForMechanicAsync(string mechanicId)
        {
            if (!int.TryParse(mechanicId, out var mid)) return 0;

            return await _ctx.Riparazioni
                             .CountAsync(r =>
                                r.MeccanicoId == mid
                             && !r.IsCompleted);
        }
    }
}
