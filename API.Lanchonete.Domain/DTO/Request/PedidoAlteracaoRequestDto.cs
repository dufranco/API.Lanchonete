namespace API.Lanchonete.Domain.DTO.Request
{
    public class PedidoAlteracaoRequestDto
    {
        public required int IdPedido { get; set; }
        public required int IdUsuario { get; set; }
        public required string Status { get; set; }
        public required List<ItemPedidoAlteracaoRequestDto> ItensPedido { get; set; }
    }

    public class ItemPedidoAlteracaoRequestDto
    {
        public required int IdItem { get; set; }
        public required int IdProduto { get; set; }
        public required int Quantidade { get; set; }
        public required string Status { get; set; }
    }
}
