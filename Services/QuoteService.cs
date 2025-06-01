using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorOfficina.Data;
using BlazorOfficina.Data.Dtos;
using BlazorOfficina.Data.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace BlazorOfficina.Services
{
    public class QuoteService : IQuoteService
    {
        private readonly ApplicationDbContext _ctx;
        private readonly IEmailSender _emailSender;
        private readonly IUserService _userService;

        public QuoteService(
            ApplicationDbContext ctx,
            IEmailSender emailSender,
            IUserService userService)
        {
            _ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<PreventivoDto> CreatePreventivoAsync(CreatePreventivoDto dto, string userId)
        {
            // Calcolo totale servizi
            var servizi = await _ctx.Servizi
                .Where(s => dto.ServiziIds.Contains(s.Id))
                .ToListAsync();
            if (!servizi.Any())
                throw new InvalidOperationException("Devi selezionare almeno un servizio.");
            decimal serviziTotal = servizi.Sum(s => s.PrezzoBase);

            // Calcolo totale ricambi
            decimal ricambiTotal = 0m;
            foreach (var item in dto.Ricambi)
            {
                var part = await _ctx.Ricambi.FindAsync(item.SparePartId)
                           ?? throw new InvalidOperationException($"Ricambio ID {item.SparePartId} non trovato.");
                ricambiTotal += part.Prezzo * item.Quantity;
            }

            // Creo Preventivo
            var entity = new Preventivo
            {
                UtenteId = int.Parse(userId),
                VeicoloId = dto.VeicoloId,
                ServizioId = dto.ServiziIds.First(),
                Titolo = string.Join(", ", servizi.Select(s => s.Nome)),
                DataCreazione = DateTime.UtcNow,
                Status = PreventivoStatus.Pending,
                Totale = serviziTotal + ricambiTotal
            };

            _ctx.Preventivi.Add(entity);
            await _ctx.SaveChangesAsync();

            // Carico relations per DTO
            await _ctx.Entry(entity).Reference(e => e.Veicolo).LoadAsync();
            await _ctx.Entry(entity).Reference(e => e.Servizio).LoadAsync();

            // Notifico meccanici e admin
            var meccanici = await _userService.GetUsersByRoleAsync("Mechanic");
            var admin = await _userService.GetUsersByRoleAsync("Admin");
            var destinatari = meccanici
                .Concat(admin)
                .Select(u => u.Email)
                .Distinct();

            var subject = "Nuovo preventivo da approvare";
            var html = $"È stato creato un nuovo preventivo <b>#{entity.Id}</b>. Vai alla dashboard per gestirlo.";
            foreach (var email in destinatari)
                await _emailSender.SendEmailAsync(email, subject, html);

            return new PreventivoDto
            {
                Id = entity.Id,
                Titolo = entity.Titolo,
                VeicoloId = entity.VeicoloId,
                ModelloVeicolo = entity.Veicolo.Modello,
                TargaVeicolo = entity.Veicolo.Targa,
                ServizioId = entity.ServizioId,
                ServizioNome = entity.Servizio.Nome,
                Totale = entity.Totale,
                DataCreazione = entity.DataCreazione,
                Status = entity.Status,
                IsConfirmed = entity.IsConfirmed
            };
        }

        public async Task<List<PreventivoDto>> GetRecentQuotesAsync(string userId, int count)
        {
            return await _ctx.Preventivi
                .AsNoTracking()
                .Include(p => p.Veicolo)
                .Include(p => p.Servizio)
                .Where(p => p.UtenteId.ToString() == userId)
                .OrderByDescending(p => p.DataCreazione)
                .Take(count)
                .Select(p => new PreventivoDto
                {
                    Id = p.Id,
                    Titolo = p.Titolo,
                    VeicoloId = p.VeicoloId,
                    ModelloVeicolo = p.Veicolo.Modello,
                    TargaVeicolo = p.Veicolo.Targa,
                    ServizioId = p.ServizioId,
                    ServizioNome = p.Servizio.Nome,
                    Totale = p.Totale,
                    DataCreazione = p.DataCreazione,
                    Status = p.Status,
                    IsConfirmed = p.IsConfirmed
                })
                .ToListAsync();
        }

        public async Task<List<PreventivoDto>> GetRecentEstimatesAsync(string userId, int count)
        {
            return await _ctx.Preventivi
                .AsNoTracking()
                .Include(p => p.Veicolo)
                .Include(p => p.Servizio)
                .Where(p => p.UtenteId.ToString() == userId)
                .OrderByDescending(p => p.DataCreazione)
                .Take(count)
                .Select(p => new PreventivoDto
                {
                    Id = p.Id,
                    Titolo = p.Titolo,
                    VeicoloId = p.VeicoloId,
                    ModelloVeicolo = p.Veicolo.Modello,
                    TargaVeicolo = p.Veicolo.Targa,
                    ServizioId = p.ServizioId,
                    ServizioNome = p.Servizio.Nome,
                    Totale = p.Totale,
                    DataCreazione = p.DataCreazione,
                    Status = p.Status
                })
                .ToListAsync();
        }

        public async Task<List<PreventivoDto>> GetPendingQuotesAsync()
        {
            return await _ctx.Preventivi
                .AsNoTracking()
                .Include(p => p.Veicolo)
                .Include(p => p.Servizio)
                .Where(p => p.Status == PreventivoStatus.Pending)
                .Select(p => new PreventivoDto
                {
                    Id = p.Id,
                    Titolo = p.Titolo,
                    VeicoloId = p.VeicoloId,
                    ModelloVeicolo = p.Veicolo.Modello,
                    TargaVeicolo = p.Veicolo.Targa,
                    ServizioId = p.ServizioId,
                    ServizioNome = p.Servizio.Nome,
                    Totale = p.Totale,
                    DataCreazione = p.DataCreazione,
                    Status = p.Status
                })
                .ToListAsync();
        }

        public async Task ConfirmQuoteAsync(int quoteId)
        {
            var entity = await _ctx.Preventivi.FindAsync(quoteId)
                         ?? throw new KeyNotFoundException($"Preventivo {quoteId} non trovato");
            entity.Status = PreventivoStatus.Confirmed;
            await _ctx.SaveChangesAsync();
        }

        public async Task<int> GetPendingQuotesCountForMechanicAsync(string mechanicId)
        {
            if (!int.TryParse(mechanicId, out var mid))
                return 0;

            return await _ctx.Preventivi
                .CountAsync(p => p.Status == PreventivoStatus.Pending);
        }

        public async Task<IEnumerable<Preventivo>> GetOpenPreventiviAsync()
        {
            return await _ctx.Preventivi
                .Include(p => p.Utente)
                .Include(p => p.Veicolo)    // ← così EF popola q.Veicolo
                .Include(p => p.Servizio)   // ← se nel markup usi anche p.Servizio.Nome
                .Where(p => p.Status == PreventivoStatus.Pending)
                .ToListAsync();
        }


        // in QuoteService.cs
        public async Task<IEnumerable<Preventivo>> GetPreventiviByMeccanicoAsync(int mechId, PreventivoStatus status)
        {
            return await _ctx.Preventivi
                .Include(p => p.Utente)
                .Include(p => p.Veicolo)    // <— aggiungi questo
                .Include(p => p.Servizio)   // <— e questo
                .Where(p => p.MeccanicoId == mechId && p.Status == status)
                .OrderByDescending(p => p.DataCreazione)
                .ToListAsync();
        }

        public async Task UpdatePreventivoStatusAsync(int preventivoId, int mechId, PreventivoStatus newStatus)
        {
            // 1) Recupera il preventivo e tutte le relazioni
            var preventivo = await _ctx.Preventivi
                .Include(p => p.Veicolo)
                .Include(p => p.Servizio)
                .Include(p => p.Utente)
                .FirstOrDefaultAsync(p => p.Id == preventivoId)
              ?? throw new KeyNotFoundException($"Preventivo {preventivoId} non trovato");

            // 2) Assegna il meccanico se non già settato
            if (preventivo.MeccanicoId == null)
                preventivo.MeccanicoId = mechId;

            // 3) Aggiorna lo status
            preventivo.Status = newStatus;
            await _ctx.SaveChangesAsync();

            // 4) Prepara testo e dettagli
            var statusText = newStatus == PreventivoStatus.Confirmed
                ? "confermato"
                : "respinto";
            var subject = $"Il tuo preventivo #{preventivo.Id} è stato {statusText}";

            var details = $@"
        <ul>
          <li><strong>Servizio:</strong> {preventivo.Servizio.Nome}</li>
          <li><strong>Veicolo:</strong> {preventivo.Veicolo.Modello} ({preventivo.Veicolo.Targa})</li>
          <li><strong>Totale stimato:</strong> {preventivo.Totale:C}</li>
        </ul>";

            var htmlMessage = $@"
        <p>Ciao {preventivo.Utente.Username},</p>
        <p>Il meccanico ha <strong>{statusText}</strong> il tuo preventivo <strong>#{preventivo.Id}</strong>.</p>
        {details}
        <p>Grazie per averci scelto,<br/>Il team di Officina</p>";

            // 5) Invia la mail
            await _emailSender.SendEmailAsync(preventivo.Utente.Email, subject, htmlMessage);
        }

        public async Task<IEnumerable<Preventivo>> GetPreventiviByClienteAsync(int clienteId, PreventivoStatus status)
        {
            return await _ctx.Preventivi               // <— qui usi _ctx e non _dbContext
                .Include(p => p.Veicolo)
                .Include(p => p.Servizio)
                .Where(p => p.UtenteId == clienteId   // ← perché nel tuo modello il FK è UtenteId
                            && p.Status == status)
                .OrderByDescending(p => p.DataCreazione)
                .ToListAsync();
        }

        public async Task<List<PreventivoDto>> GetAllEstimatesForMechanicAsync(string mechanicId)
        {
            if (!int.TryParse(mechanicId, out var mid))
                return new List<PreventivoDto>();

            return await _ctx.Preventivi
                .Where(p => p.MeccanicoId == mid)
                .OrderByDescending(p => p.DataCreazione)
                .Select(p => new PreventivoDto
                {
                    Id = p.Id,
                    Titolo = p.Titolo,
                    ModelloVeicolo = p.Veicolo.Modello,
                    TargaVeicolo = p.Veicolo.Targa,
                    Totale = p.Totale,
                    Status = (PreventivoStatus)p.Status,      // o la tua logica di mapping
                    DataCreazione = p.DataCreazione
                })
                .ToListAsync();
        }

        public async Task<List<Preventivo>> GetPreventiviByStatusAsync(PreventivoStatus status)
        {
            return await _ctx.Preventivi
                            .Include(p => p.Utente)     // carica il cliente
                            .Include(p => p.Veicolo)    // carica il veicolo
                            .Include(p => p.Servizio)   // carica il servizio
                            .Where(p => p.Status == status)
                            .ToListAsync();
        }

    }
}
