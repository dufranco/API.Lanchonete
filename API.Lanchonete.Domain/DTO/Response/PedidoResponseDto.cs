using API.Lanchonete.Domain.DTO.Abstract;

namespace API.Lanchonete.Domain.DTO.Response
{
    public class PedidoResponseDto : PedidoBase
    {
        public required int IdPedido { get; set; }
        public required string NomeUsuario { get; set; }
        public required string Status { get; set; }
        public required DateTime DataCadastro { get; set; }
        public required List<ItemPedidoResponseDto> ItensPedido { get; set; }        
    }

    public class ItemPedidoResponseDto : ItemPedidoBase
    {
        public required int IdItem { get; set; }
        public required string DescricaoProduto { get; set; }
        public required string Status { get; set; }
    }
}
