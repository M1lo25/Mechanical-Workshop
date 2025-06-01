using BlazorOfficina.Data.Models;

public class PreventivoDto
{
    public int Id { get; set; }
    public string Titolo { get; set; } = string.Empty;
    public int VeicoloId { get; set; }
    public string ModelloVeicolo { get; set; } = string.Empty;
    public string TargaVeicolo { get; set; } = string.Empty;

    // Rinomina pure se vuoi, ma assicurati che combacino con il mapping:
    public int ServizioId { get; set; }
    public string ServizioNome { get; set; } = string.Empty;

    public decimal Totale { get; set; }
    public DateTime DataCreazione { get; set; }

   // <-- aggiungi questa proprietà
    public PreventivoStatus Status { get; set; }

    public bool IsConfirmed { get; set; }
}
