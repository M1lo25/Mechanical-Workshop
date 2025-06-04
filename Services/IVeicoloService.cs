using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorOfficina.Data.Dtos;

namespace BlazorOfficina.Services
{
    public interface IVeicoloService
    {
        Task<VeicoloDto> CreaVeicoloAsync(string userId, AggiungiVeicoloDto dto);
        Task<VeicoloDto?> GetByIdAsync(int id);
        Task<int> GetRegisteredVehiclesCountAsync(string userId);
        Task<List<VeicoloDto>> GetUserVehiclesAsync(string userId);
        Task<List<VeicoloDto>> GetVehiclesPendingAsync();
        Task<List<ServiceHistoryDto>> GetServiceHistoryAsync(int veicoloId);
        Task<int> GetPendingVehiclesCountForMechanicAsync(string mechanicId);

        Task<List<MechanicVehicleDto>> GetAllWithCustomerAsync();
        Task DeleteAsync(string userId, int id);
        Task DeleteVehicleAsync(string userId, int id);
    }
}
