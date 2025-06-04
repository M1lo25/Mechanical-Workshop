using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using BlazorOfficina.Data;
using BlazorOfficina.Data.Dtos;
using BlazorOfficina.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BlazorOfficina.Services
{
    public class VeicoloService : IVeicoloService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _http;

        public VeicoloService(
            ApplicationDbContext db,
            IMapper mapper,
            IHttpContextAccessor http)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _http = http ?? throw new ArgumentNullException(nameof(http));
        }

        public async Task<VeicoloDto> CreaVeicoloAsync(string userId, AggiungiVeicoloDto dto)
        {
            if (!int.TryParse(userId, out var uid))
                throw new InvalidOperationException("Utente non autenticato.");

            var entity = _mapper.Map<Veicolo>(dto);
            entity.UtenteId = uid;

            _db.Veicoli.Add(entity);
            await _db.SaveChangesAsync();

            return _mapper.Map<VeicoloDto>(entity);
        }
        public async Task<VeicoloDto?> GetByIdAsync(int id)
        {
            var v = await _db.Veicoli.FindAsync(id);
            if (v is null) return null;
            return _mapper.Map<VeicoloDto>(v);
        }

        public async Task<int> GetRegisteredVehiclesCountAsync(string userId)
        {
            if (!int.TryParse(userId, out var uid))
                return 0;

            return await _db.Veicoli
                            .AsNoTracking()
                            .CountAsync(v => v.UtenteId == uid);
        }

        public async Task<List<VeicoloDto>> GetUserVehiclesAsync(string userId)
        {
            if (!int.TryParse(userId, out var uid))
                return new List<VeicoloDto>();

            return await _db.Veicoli
                .AsNoTracking()
                .Where(v => v.UtenteId == uid)
                .Select(v => new VeicoloDto
                {
                    Id = v.Id,
                    Categoria = v.Categoria,
                    Marca = v.Marca,
                    Modello = v.Modello,
                    Anno = v.Anno,
                    Targa = v.Targa,
                    Chilometraggio = v.Chilometraggio,
                    Carburante = v.Carburante,
                    Note = v.Note,
                    LibrettoCircolazione = v.LibrettoCircolazione,
                    ManualeUso = v.ManualeUso,
                    ChiaviDiRiserva = v.ChiaviDiRiserva,
                    UtenteId = v.UtenteId,
                    Stato = v.Stato,
                    PercentualeRiparazione = v.PercentualeRiparazione,
                    InterventoCorrente = v.InterventoCorrente,
                    DataInizio = v.DataInizio,
                    DataPrevisto = v.DataPrevisto,
                    UltimoIntervento = v.Riparazioni
                           .OrderByDescending(r => r.DataProntoRitiro)
                           .Select(r => r.DataProntoRitiro)
                           .FirstOrDefault(),
                    SpesaTotale = v.Riparazioni.Sum(r => r.Costo)
                })
                .ToListAsync();
        }

        public async Task<List<VeicoloDto>> GetVehiclesPendingAsync()
        {
            var list = await _db.Veicoli
                .Where(v => v.Riparazioni.Any(r => !r.IsCompleted))
                .Distinct()
                .ToListAsync();

            return _mapper.Map<List<VeicoloDto>>(list);
        }

        public async Task<List<ServiceHistoryDto>> GetServiceHistoryAsync(int veicoloId)
        {
            return await _db.Riparazioni
                .Where(r => r.VeicoloId == veicoloId && r.IsCompleted)
                .OrderByDescending(r => r.DataProntoRitiro)
                .Select(r => new ServiceHistoryDto
                {
                    Descrizione = r.Descrizione,
                    Data = r.DataProntoRitiro,
                    Kilometri = r.Chilometraggio,
                    Costo = r.Costo
                })
                .ToListAsync();
        }

        public async Task<int> GetPendingVehiclesCountForMechanicAsync(string mechanicId)
        {
            if (!int.TryParse(mechanicId, out var mid))
                return 0;

            return await _db.Appuntamenti
                .Where(a => a.MeccanicoId == mid
                         && a.Stato == StatoAppuntamento.InAttesa)
                .Select(a => a.VeicoloId)   
                .Distinct()                 
                .CountAsync();
        }

        public async Task<List<MechanicVehicleDto>> GetAllWithCustomerAsync()
        {
            return await _db.Veicoli
                .Include(v => v.Utente)
                .Include(v => v.Appuntamenti)
                .Include(v => v.Riparazioni)
                .Select(v => new MechanicVehicleDto
                {
                    VehicleId = v.Id,
                    CustomerName = v.Utente.Username,
                    CustomerEmail = v.Utente.Email,
                    Marca = v.Marca,
                    Modello = v.Modello,
                    Targa = v.Targa,
                    Anno = v.Anno,
                    TipoCarburante = v.Carburante.ToString(),
                    Chilometraggio = v.Chilometraggio,
                    Stato = v.Appuntamenti
                                            .OrderByDescending(a => a.Inizio)
                                            .Select(a => a.Stato.ToString())
                                            .FirstOrDefault() ?? "Nessuno",
                    ProgressPercentage = v.Riparazioni.Count == 0
                                            ? 0
                                            : (v.Riparazioni.Count(r => r.IsCompleted) * 100)
                                              / v.Riparazioni.Count
                })
                .ToListAsync();
        }

        public async Task DeleteAsync(string userId, int id)
        {
            // opzionalmente: controlla se (v.UtenteId == userId) -> autorizzazione
            var entity = await _db.Veicoli.FindAsync(id)
                         ?? throw new KeyNotFoundException($"Veicolo {id} non trovato");
            _db.Veicoli.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public Task DeleteVehicleAsync(string userId, int id)
            => DeleteAsync(userId, id);

    }
}
