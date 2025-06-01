public enum AppointmentStatus
{
    InAttesa,
    InLavorazione,
    Completato
}

public class MechanicAppointmentDto
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string ClienteNome { get; set; } = null!;
    public int VeicoloId { get; set; }
    public string VeicoloTarga { get; set; } = null!;
    public string ServizioNome { get; set; } = null!;     // ← nuovo
    public DateTime Inizio { get; set; }
    public DateTime Fine { get; set; }
    public decimal CostoStimato { get; set; }
    public AppointmentStatus Stato { get; set; }         // ← nuovo
}
