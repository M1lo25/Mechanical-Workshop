using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BlazorOfficina.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PasswordResetTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Expiry = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ricambi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Codice = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Prezzo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ricambi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Servizi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrezzoBase = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DurataMinuti = table.Column<int>(type: "int", nullable: false),
                    DurataMassimi = table.Column<int>(type: "int", nullable: false),
                    Descrizione = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servizi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegisteredAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ProfileImage = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Veicoli",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Categoria = table.Column<int>(type: "int", nullable: false),
                    Marca = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Modello = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Anno = table.Column<int>(type: "int", nullable: false),
                    Targa = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Chilometraggio = table.Column<int>(type: "int", nullable: false),
                    Carburante = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LibrettoCircolazione = table.Column<bool>(type: "bit", nullable: false),
                    ManualeUso = table.Column<bool>(type: "bit", nullable: false),
                    ChiaviDiRiserva = table.Column<bool>(type: "bit", nullable: false),
                    UtenteId = table.Column<int>(type: "int", nullable: false),
                    Stato = table.Column<int>(type: "int", nullable: false),
                    PercentualeRiparazione = table.Column<int>(type: "int", nullable: false),
                    InterventoCorrente = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DataInizio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataPrevisto = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veicoli", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Veicoli_Users_UtenteId",
                        column: x => x.UtenteId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Appuntamenti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MeccanicoId = table.Column<int>(type: "int", nullable: false),
                    UtenteId = table.Column<int>(type: "int", nullable: false),
                    VeicoloId = table.Column<int>(type: "int", nullable: false),
                    ServizioId = table.Column<int>(type: "int", nullable: false),
                    Inizio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Fine = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModalitaConsegna = table.Column<int>(type: "int", nullable: false),
                    PreferenzeContatto = table.Column<int>(type: "int", nullable: false),
                    CostoStimato = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Stato = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appuntamenti", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appuntamenti_Servizi_ServizioId",
                        column: x => x.ServizioId,
                        principalTable: "Servizi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appuntamenti_Users_MeccanicoId",
                        column: x => x.MeccanicoId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appuntamenti_Users_UtenteId",
                        column: x => x.UtenteId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appuntamenti_Veicoli_VeicoloId",
                        column: x => x.VeicoloId,
                        principalTable: "Veicoli",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Preventivi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UtenteId = table.Column<int>(type: "int", nullable: false),
                    VeicoloId = table.Column<int>(type: "int", nullable: false),
                    Titolo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ServizioId = table.Column<int>(type: "int", nullable: false),
                    Totale = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DataCreazione = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    MeccanicoId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Preventivi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Preventivi_Servizi_ServizioId",
                        column: x => x.ServizioId,
                        principalTable: "Servizi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Preventivi_Users_MeccanicoId",
                        column: x => x.MeccanicoId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Preventivi_Users_UtenteId",
                        column: x => x.UtenteId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Preventivi_Veicoli_VeicoloId",
                        column: x => x.VeicoloId,
                        principalTable: "Veicoli",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Riparazioni",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UtenteId = table.Column<int>(type: "int", nullable: false),
                    MeccanicoId = table.Column<int>(type: "int", nullable: false),
                    VeicoloId = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    DataProntoRitiro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descrizione = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Chilometraggio = table.Column<int>(type: "int", nullable: false),
                    Costo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Riparazioni", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Riparazioni_Users_MeccanicoId",
                        column: x => x.MeccanicoId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Riparazioni_Users_UtenteId",
                        column: x => x.UtenteId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Riparazioni_Veicoli_VeicoloId",
                        column: x => x.VeicoloId,
                        principalTable: "Veicoli",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Ricambi",
                columns: new[] { "Id", "Codice", "Nome", "Prezzo" },
                values: new object[,]
                {
                    { 1, "FR123", "Pastiglie freni anteriori", 45m },
                    { 2, "FO456", "Filtro olio", 15m },
                    { 3, "BT789", "Batteria 12V 60Ah", 90m },
                    { 4, "PN012", "Pneumatico 195/65 R15", 75m },
                    { 5, "KD345", "Kit distribuzione", 120m },
                    { 6, "LP678", "Lampadina faretto", 8m }
                });

            migrationBuilder.InsertData(
                table: "Servizi",
                columns: new[] { "Id", "Descrizione", "DurataMassimi", "DurataMinuti", "Nome", "PrezzoBase" },
                values: new object[,]
                {
                    { 1, "", 90, 60, "Tagliando", 100m },
                    { 2, "", 105, 75, "Revisione", 150m },
                    { 3, "", 120, 90, "Riparazione", 200m },
                    { 4, "Tagliando + controlli extra", 110, 80, "Tagliando Completo", 180m },
                    { 5, "Pastiglie + lavoro montaggio", 90, 60, "Sostituzione freni", 250m },
                    { 6, "Lettura codici errore ECU", 45, 30, "Diagnostica computerizzata", 80m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appuntamenti_MeccanicoId",
                table: "Appuntamenti",
                column: "MeccanicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Appuntamenti_ServizioId",
                table: "Appuntamenti",
                column: "ServizioId");

            migrationBuilder.CreateIndex(
                name: "IX_Appuntamenti_UtenteId",
                table: "Appuntamenti",
                column: "UtenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Appuntamenti_VeicoloId",
                table: "Appuntamenti",
                column: "VeicoloId");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_Token",
                table: "PasswordResetTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Preventivi_MeccanicoId",
                table: "Preventivi",
                column: "MeccanicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Preventivi_ServizioId",
                table: "Preventivi",
                column: "ServizioId");

            migrationBuilder.CreateIndex(
                name: "IX_Preventivi_UtenteId",
                table: "Preventivi",
                column: "UtenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Preventivi_VeicoloId",
                table: "Preventivi",
                column: "VeicoloId");

            migrationBuilder.CreateIndex(
                name: "IX_Riparazioni_MeccanicoId",
                table: "Riparazioni",
                column: "MeccanicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Riparazioni_UtenteId",
                table: "Riparazioni",
                column: "UtenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Riparazioni_VeicoloId",
                table: "Riparazioni",
                column: "VeicoloId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Veicoli_UtenteId",
                table: "Veicoli",
                column: "UtenteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appuntamenti");

            migrationBuilder.DropTable(
                name: "PasswordResetTokens");

            migrationBuilder.DropTable(
                name: "Preventivi");

            migrationBuilder.DropTable(
                name: "Ricambi");

            migrationBuilder.DropTable(
                name: "Riparazioni");

            migrationBuilder.DropTable(
                name: "Servizi");

            migrationBuilder.DropTable(
                name: "Veicoli");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
