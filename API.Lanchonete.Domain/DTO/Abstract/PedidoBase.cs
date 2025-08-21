namespace API.Lanchonete.Domain.DTO.Abstract
{
    public abstract class PedidoBase
    {
        public required int IdUsuario { get; set; }
    }

    public abstract class ItemPedidoBase
    {
        public int IdProduto { get; set; }
        public int Quantidade { get; set; }
    }
}
