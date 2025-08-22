using API.Lanchonete.Domain.DTO.Response;
using API.Lanchonete.Domain.Entities;
using AutoMapper;

namespace API.Lanchonete.Data.Mappings.AutoMapper
{
    public class ControleAcessoToControleAcessoResponseDtoMappingProfile : Profile
    {
        public ControleAcessoToControleAcessoResponseDtoMappingProfile()
        {
            CreateMap<ControleAcesso, ControleAcessoResponseDto>()
                .ForMember(dest => dest.DataCadastro, opt => opt.MapFrom(src => src.DataCadastro))
                .ForMember(dest => dest.IdControle, opt => opt.MapFrom(src => src.IdControle))
                .ForMember(dest => dest.IdPerfil, opt => opt.MapFrom(src => src.IdPerfil))
                .ForMember(dest => dest.NomeTela, opt => opt.MapFrom(src => src.NomeTela))
                .ForMember(dest => dest.NomePerfil, opt => opt.MapFrom(src => src.IdPerfilNavigation.Nome))
                .ForMember(dest => dest.Permitido, opt => opt.MapFrom(src => src.Permitido));
        }
    }
}
