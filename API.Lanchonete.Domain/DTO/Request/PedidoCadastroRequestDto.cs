namespace API.Lanchonete.Domain.DTO.Request
{
    public class PedidoCadastroRequestDto
    {
        public required int IdUsuario { get; set; }
        public required List<ItemPedidoCadastroRequestDto> ItensPedido { get; set; }
    }

    public class ItemPedidoCadastroRequestDto
    {
        public int IdProduto { get; set; }
        public int Quantidade { get; set; }
    }
}
