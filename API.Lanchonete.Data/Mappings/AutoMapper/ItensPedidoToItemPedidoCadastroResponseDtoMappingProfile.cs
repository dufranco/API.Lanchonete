using API.Lanchonete.Domain.DTO.Response;
using API.Lanchonete.Domain.Entities;
using AutoMapper;

namespace API.Lanchonete.Data.Mappings.AutoMapper
{
    public class ItensPedidoToItemPedidoCadastroResponseDtoMappingProfile : Profile
    {
        public ItensPedidoToItemPedidoCadastroResponseDtoMappingProfile()
        {
            CreateMap<ItensPedido, ItemPedidoCadastroResponseDto>()
                .ReverseMap();
        }
    }
}
