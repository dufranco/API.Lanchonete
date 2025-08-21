using API.Lanchonete.Domain.DTO.Request;
using API.Lanchonete.Domain.Entities;
using AutoMapper;

namespace API.Lanchonete.Data.Mappings.AutoMapper
{
    public class PedidoAlteracaoRequestDtoToPedidoMappingProfile : Profile
    {
        public PedidoAlteracaoRequestDtoToPedidoMappingProfile()
        {
            CreateMap<PedidoAlteracaoRequestDto, Pedido>()
                .ReverseMap();
        }
    }
}
