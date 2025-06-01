// File: Data/Dtos/MechanicVehicleDto.cs
namespace BlazorOfficina.Data.Dtos
{
    public class MechanicVehicleDto
    {
        public int VehicleId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;

        public string Marca { get; set; } = string.Empty;
        public string Modello { get; set; } = string.Empty;
        public string Targa { get; set; } = string.Empty;
        public int Anno { get; set; }
        public string TipoCarburante { get; set; } = string.Empty;
        public int? Chilometraggio { get; set; }

        public string Stato { get; set; } = string.Empty;
        public int ProgressPercentage { get; set; }
    }
}
