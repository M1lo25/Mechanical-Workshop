using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorOfficina.Data.Dtos;

namespace BlazorOfficina.Services
{
    public interface IRepairService
    {
        /// <summary>
        /// Recupera tutte le riparazioni attualmente in corso.
        /// </summary>
        Task<List<RiparazioneDto>> GetOngoingRepairsAsync();

        /// <summary>
        /// Recupera i dettagli di una singola riparazione.
        /// </summary>
        Task<RiparazioneDto> GetRepairByIdAsync(int id);
        Task<IEnumerable<RiparazioneDto>> GetByMechanicAsync(string userId);
        Task<int> GetInProgressCountForMechanicAsync(string mechanicId);
    }
}
