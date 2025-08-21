using API.Lanchonete.Domain.DTO.Abstract;

namespace API.Lanchonete.Domain.DTO.Response
{
    public class PedidoCadastroResponseDto : PedidoBase
    {
        public required int IdPedido { get; set; }
        public required string Status { get; set; }
        public required List<ItemPedidoCadastroResponseDto> ItensPedido { get; set; }
    }

    public class ItemPedidoCadastroResponseDto : ItemPedidoBase
    {
        public required int IdItem { get; set; }
        public required string Status { get; set; }
    }
}
