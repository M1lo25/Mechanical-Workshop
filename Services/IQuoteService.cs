using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorOfficina.Data.Dtos;
using BlazorOfficina.Data.Models;

namespace BlazorOfficina.Services
{

    public interface IQuoteService
    {
        Task<PreventivoDto> CreatePreventivoAsync(CreatePreventivoDto dto, string userId);
        Task<List<PreventivoDto>> GetRecentEstimatesAsync(string userId, int count);
        Task<List<PreventivoDto>> GetRecentQuotesAsync(string userId, int count = 5);
        Task<IEnumerable<Preventivo>> GetOpenPreventiviAsync();
        Task<IEnumerable<Preventivo>> GetPreventiviByMeccanicoAsync(int mechId, PreventivoStatus status);
        Task UpdatePreventivoStatusAsync(int preventivoId, int mechId, PreventivoStatus newStatus);
        Task<int> GetPendingQuotesCountForMechanicAsync(string mechanicId);
        Task<List<PreventivoDto>> GetPendingQuotesAsync();
        Task ConfirmQuoteAsync(int quoteId);
        Task<List<PreventivoDto>> GetAllEstimatesForMechanicAsync(string mechanicId);
        Task<IEnumerable<Preventivo>> GetPreventiviByClienteAsync(int clienteId, PreventivoStatus status);
        Task<List<Preventivo>> GetPreventiviByStatusAsync(PreventivoStatus status);
    }
}
