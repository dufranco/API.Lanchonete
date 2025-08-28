using API.Lanchonete.Domain.DTO.Response;
using API.Lanchonete.Domain.Entities;
using AutoMapper;

namespace API.Lanchonete.Data.Mappings.AutoMapper
{
    public class PedidoToPedidoResponseDtoMappingProfile : Profile
    {
        public PedidoToPedidoResponseDtoMappingProfile()
        {
            CreateMap<Pedido, PedidoResponseDto>()
                .ForMember(dest => dest.IdPedido, opt => opt.MapFrom(src => src.IdPedido))
                .ForMember(dest => dest.NomeUsuario, opt => opt.MapFrom(src => src.IdUsuarioNavigation.Nome))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.IdUsuario, opt => opt.MapFrom(src => src.IdUsuario))
                .ForMember(dest => dest.DataCadastro, opt => opt.MapFrom(src => src.DataCadastro))
                .ForMember(dest => dest.ItensPedido, opt => opt.MapFrom(src => ItensPedidoToItemPedidoListagemResponseDtoList(src.ItensPedidos.ToList())));
        }

        private static List<ItemPedidoResponseDto> ItensPedidoToItemPedidoListagemResponseDtoList(List<ItensPedido> itensPedido)
            => itensPedido.Select(ip => new ItemPedidoResponseDto()
                   {
                       IdItem = ip.IdItem,
                       IdProduto = ip.IdProduto,
                       DescricaoProduto = ip.IdProdutoNavigation.Nome,
                       Quantidade = ip.Quantidade,
                       Status = ip.Status
                   }
               ).ToList();
    }
}
