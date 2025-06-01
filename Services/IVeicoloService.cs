using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorOfficina.Data.Dtos;

namespace BlazorOfficina.Services
{
    public interface IVeicoloService
    {
        Task<VeicoloDto> CreaVeicoloAsync(AggiungiVeicoloDto dto);
        Task<VeicoloDto?> GetByIdAsync(int id);
        Task<int> GetRegisteredVehiclesCountAsync(string userId);
        Task<List<VeicoloDto>> GetUserVehiclesAsync(string userId);
        Task<List<VeicoloDto>> GetVehiclesPendingAsync();
        Task<List<ServiceHistoryDto>> GetServiceHistoryAsync(int veicoloId);
        Task<int> GetPendingVehiclesCountForMechanicAsync(string mechanicId);

        // Pagina meccanico: veicoli + dati cliente + stato + % completamento
        Task<List<MechanicVehicleDto>> GetAllWithCustomerAsync();

        // Metodo per cancellare un veicolo
        Task DeleteAsync(int id);
        Task DeleteVehicleAsync(int id);
    }
}
