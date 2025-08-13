using API.Lanchonete.Domain.DTO.Request;
using API.Lanchonete.Domain.Entities;
using AutoMapper;

namespace API.Lanchonete.Data.Mappings.AutoMapper
{
    public class ProdutoAlteracaoRequestDtoToProdutoMappingProfile : Profile
    {
        public ProdutoAlteracaoRequestDtoToProdutoMappingProfile()
        {
            CreateMap<ProdutoAlteracaoRequestDto, Produto>()
                .ReverseMap();
        }
    }
}
