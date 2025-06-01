using AutoMapper;
using BlazorOfficina.Data.Dtos;
using BlazorOfficina.Data.Models;

namespace BlazorOfficina.Mapping
{
    public class VeicoloProfile : Profile
    {
        public VeicoloProfile()
        {
            // mappa dal DTO di input all’entità:
            CreateMap<AggiungiVeicoloDto, Veicolo>();

            // mappa dall’entità al DTO di output:
            CreateMap<Veicolo, VeicoloDto>();
        }
    }
}
