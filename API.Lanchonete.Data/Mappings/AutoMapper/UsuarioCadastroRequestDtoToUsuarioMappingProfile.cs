using API.Lanchonete.Core.Utils;
using API.Lanchonete.Domain.DTO.Request;
using API.Lanchonete.Domain.Entities;
using AutoMapper;
using System.Security;

namespace API.Lanchonete.Data.Mappings.AutoMapper
{
    public class UsuarioCadastroRequestDtoToUsuarioMappingProfile : Profile
    {
        private readonly byte[] _salt;

        public UsuarioCadastroRequestDtoToUsuarioMappingProfile()
        {
            _salt = SenhaSaltHash.GerarSalt();

            CreateMap<UsuarioCadastroRequestDto, Usuario>()
                .ForMember(Usuario => Usuario.Nome, opt => opt.MapFrom(src => src.Nome))
                .ForMember(Usuario => Usuario.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(Usuario => Usuario.IdPerfil, opt => opt.MapFrom(src => src.IdPerfil))
                .ForMember(Usuario => Usuario.SenhaSalt, opt => opt.MapFrom(src => Convert.ToBase64String(_salt)))
                .ForMember(Usuario => Usuario.SenhaHash, opt => opt.MapFrom(src => GerarHashSenha(src.SenhaCriptografada!, _salt)));
        }

        private static string GerarHashSenha(SecureString senha, byte[] salt)
            => Convert.ToBase64String(SenhaSaltHash.GerarHashSenha(senha, salt));
    }
}
