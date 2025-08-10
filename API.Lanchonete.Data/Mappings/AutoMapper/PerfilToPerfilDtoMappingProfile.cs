using API.Lanchonete.Domain.DTO;
using API.Lanchonete.Domain.Entities;
using AutoMapper;

namespace API.Lanchonete.Data.Mappings.AutoMapper
{
    public class PerfilToPerfilDtoMappingProfile : Profile
    {
        public PerfilToPerfilDtoMappingProfile()
        {
            CreateMap<Perfil, PerfilDto>()
                .ReverseMap();
        }
    }
}
