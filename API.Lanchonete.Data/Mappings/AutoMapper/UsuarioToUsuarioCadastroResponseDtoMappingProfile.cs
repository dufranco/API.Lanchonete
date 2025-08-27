using API.Lanchonete.Domain.DTO.Response;
using API.Lanchonete.Domain.Entities;
using AutoMapper;

namespace API.Lanchonete.Data.Mappings.AutoMapper
{
    public class UsuarioToUsuarioCadastroResponseDtoMappingProfile : Profile
    {
        public UsuarioToUsuarioCadastroResponseDtoMappingProfile()
        {
            CreateMap<Usuario, UsuarioCadastroResponseDto>()
                .ForMember(Usuario => Usuario.IdUsuario, opt => opt.MapFrom(src => src.IdUsuario))
                .ForMember(Usuario => Usuario.Nome, opt => opt.MapFrom(src => src.Nome))
                .ForMember(Usuario => Usuario.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(Usuario => Usuario.IdPerfil, opt => opt.MapFrom(src => src.IdPerfil))
                .ForMember(Usuario => Usuario.NomePerfil, opt => opt.MapFrom(src => src.IdPerfilNavigation.Nome))
                .ForMember(Usuario => Usuario.ControleAcessos, opt => opt.MapFrom(src => src.IdPerfilNavigation.ControleAcessos));
        }
    }
}
