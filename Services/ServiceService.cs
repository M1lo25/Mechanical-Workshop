using AutoMapper;
using BlazorOfficina.Data;
using BlazorOfficina.Data.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorOfficina.Services
{
    public class ServiceService : IServiceService
    {
        private readonly ApplicationDbContext _ctx;
        private readonly IMapper _mapper;

        public ServiceService(ApplicationDbContext ctx, IMapper mapper)
        {
            _ctx = ctx;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ServiceDto>> GetAllAsync()
        {
            var entities = await _ctx.Servizi.ToListAsync();
            return entities.Select(s =>
                new ServiceDto(
                s.Id,
                s.Nome,
                s.PrezzoBase,
                s.Descrizione,                                    // <— la descrizione dell’entità
                $"{s.DurataMinuti}–{s.DurataMassimi} min"         // <— DurationRange
                )
            );
        }
    }
}
