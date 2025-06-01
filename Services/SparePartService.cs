using BlazorOfficina.Data;
using BlazorOfficina.Data.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorOfficina.Services
{
    public class SparePartService : ISparePartService
    {
        private readonly ApplicationDbContext _ctx;

        public SparePartService(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IEnumerable<SparePartDto>> GetAllAsync()
        {
            return await _ctx.Ricambi
                .Select(r => new SparePartDto(
                    r.Id,
                    r.Nome,
                    r.Codice,
                    r.Prezzo
                ))
                .ToListAsync();
        }
    }
}
