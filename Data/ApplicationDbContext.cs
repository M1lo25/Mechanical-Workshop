using BlazorOfficina.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorOfficina.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Utenti
        public DbSet<User> Users { get; set; }

        // Token per reset password
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        // Entità del dominio officina
        public DbSet<Veicolo> Veicoli { get; set; }
        public DbSet<Riparazione> Riparazioni { get; set; }
        public DbSet<Appuntamento> Appuntamenti { get; set; }
        public DbSet<Preventivo> Preventivi { get; set; }
        public DbSet<Servizio> Servizi { get; set; }
        public DbSet<Ricambio> Ricambi { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // --- Seeding di Servizi e Ricambi ---
            builder.Entity<Servizio>().HasData(
                new { Id = 1, Nome = "Tagliando", PrezzoBase = 100m, DurataMinuti = 60, DurataMassimi = 90, Descrizione = "" },
                new { Id = 2, Nome = "Revisione", PrezzoBase = 150m, DurataMinuti = 75, DurataMassimi = 105, Descrizione = "" },
                new { Id = 3, Nome = "Riparazione", PrezzoBase = 200m, DurataMinuti = 90, DurataMassimi = 120, Descrizione = "" },
                new { Id = 4, Nome = "Tagliando Completo", PrezzoBase = 180m, DurataMinuti = 80, DurataMassimi = 110, Descrizione = "Tagliando + controlli extra" },
                new { Id = 5, Nome = "Sostituzione freni", PrezzoBase = 250m, DurataMinuti = 60, DurataMassimi = 90, Descrizione = "Pastiglie + lavoro montaggio" },
                new { Id = 6, Nome = "Diagnostica computerizzata", PrezzoBase = 80m, DurataMinuti = 30, DurataMassimi = 45, Descrizione = "Lettura codici errore ECU" }
            );

            builder.Entity<Ricambio>().HasData(
                new { Id = 1, Nome = "Pastiglie freni anteriori", Codice = "FR123", Prezzo = 45m },
                new { Id = 2, Nome = "Filtro olio", Codice = "FO456", Prezzo = 15m },
                new { Id = 3, Nome = "Batteria 12V 60Ah", Codice = "BT789", Prezzo = 90m },
                new { Id = 4, Nome = "Pneumatico 195/65 R15", Codice = "PN012", Prezzo = 75m },
                new { Id = 5, Nome = "Kit distribuzione", Codice = "KD345", Prezzo = 120m },
                new { Id = 6, Nome = "Lampadina faretto", Codice = "LP678", Prezzo = 8m }
            );

            builder.Entity<Ricambio>(entity =>
            {
                entity.Property(r => r.Prezzo)
                      .HasPrecision(18, 2);
            });

            // --- Users ---
            builder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.RegisteredAt)
                      .HasDefaultValueSql("GETUTCDATE()");
                entity.Property(u => u.ProfileImage)
                      .HasColumnType("varbinary(max)");
            });

            // --- Password reset token ---
            builder.Entity<PasswordResetToken>()
                   .HasIndex(t => t.Token)
                   .IsUnique();

            // --- Veicolo → Utente (cascade delete) ---
            builder.Entity<Veicolo>(entity =>
            {
                entity.HasOne(v => v.Utente)
                      .WithMany()
                      .HasForeignKey(v => v.UtenteId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // --- Riparazione → Utente, Meccanico, Veicolo ---
            builder.Entity<Riparazione>(entity =>
            {
                // Cliente → Riparazioni (cascade)
                entity.HasOne(r => r.Utente)
                      .WithMany()
                      .HasForeignKey(r => r.UtenteId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Meccanico → Riparazioni (restrict)
                entity.HasOne(r => r.Meccanico)
                      .WithMany()
                      .HasForeignKey(r => r.MeccanicoId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Veicolo → Riparazioni (restrict)
                entity.HasOne(r => r.Veicolo)
                      .WithMany(v => v.Riparazioni)
                      .HasForeignKey(r => r.VeicoloId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Precisione del campo Costo
                entity.Property(r => r.Costo)
                      .HasPrecision(18, 2);
            });

            // --- Appuntamento → Utente, Meccanico, Veicolo, Servizio ---
            builder.Entity<Appuntamento>(entity =>
            {
                // Utente (cliente) → Appuntamenti
                entity.HasOne(a => a.Utente)
                      .WithMany()
                      .HasForeignKey(a => a.UtenteId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Meccanico → Appuntamenti
                entity.HasOne(a => a.Meccanico)
                      .WithMany()
                      .HasForeignKey(a => a.MeccanicoId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Veicolo → Appuntamenti
                entity.HasOne(a => a.Veicolo)
                      .WithMany(v => v.Appuntamenti)        // ← qui, punta a Veicolo.Appuntamenti
                      .HasForeignKey(a => a.VeicoloId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Servizio → Appuntamenti
                entity.HasOne(a => a.Servizio)
                      .WithMany()
                      .HasForeignKey(a => a.ServizioId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Precisione su CostoStimato
                entity.Property(a => a.CostoStimato)
                      .HasPrecision(18, 2);
            });

            // --- Servizio: precisione prezzo ---
            builder.Entity<Servizio>(entity =>
            {
                entity.Property(s => s.PrezzoBase)
                      .HasPrecision(18, 2);
            });

            // --- Preventivo → Utente, Veicolo, Meccanico ---
            builder.Entity<Preventivo>(entity =>
            {
                // Utente (cliente) → Preventivi
                entity.HasOne(p => p.Utente)
                      .WithMany()
                      .HasForeignKey(p => p.UtenteId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Meccanico → Preventivi
                entity.HasOne(p => p.Meccanico)
                      .WithMany()
                      .HasForeignKey(p => p.MeccanicoId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Veicolo → Preventivi
                entity.HasOne(p => p.Veicolo)
                      .WithMany()
                      .HasForeignKey(p => p.VeicoloId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Precisione sul Totale
                entity.Property(p => p.Totale)
                      .HasPrecision(18, 2);
            });
        }
    }
}
