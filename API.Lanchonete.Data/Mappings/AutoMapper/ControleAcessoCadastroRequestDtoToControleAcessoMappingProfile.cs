using API.Lanchonete.Domain.DTO.Request;
using API.Lanchonete.Domain.Entities;
using AutoMapper;

namespace API.Lanchonete.Data.Mappings.AutoMapper
{
    public class ControleAcessoCadastroRequestDtoToControleAcessoMappingProfile : Profile
    {
        public ControleAcessoCadastroRequestDtoToControleAcessoMappingProfile()
        {
            CreateMap<ControleAcessoCadastroRequestDto, ControleAcesso>()
                .ForMember(dest => dest.IdPerfil, opt => opt.MapFrom(src => src.IdPerfil))
                .ForMember(dest => dest.NomeTela, opt => opt.MapFrom(src => src.NomeTela))
                .ForMember(dest => dest.Permitido, opt => opt.MapFrom(src => src.Permitido));
        }
    }
}
