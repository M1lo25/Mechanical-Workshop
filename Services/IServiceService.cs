using BlazorOfficina.Data.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorOfficina.Services
{
    public interface IServiceService
    {
        Task<IEnumerable<ServiceDto>> GetAllAsync();
    }
}
