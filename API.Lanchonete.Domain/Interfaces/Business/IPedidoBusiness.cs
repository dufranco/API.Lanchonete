using API.Lanchonete.Domain.DTO.Request;
using API.Lanchonete.Domain.DTO.Request.Filtro;
using API.Lanchonete.Domain.DTO.Response;

namespace API.Lanchonete.Domain.Interfaces.Business
{
    public interface IPedidoBusiness
    {
        Task<PedidoCadastroResponseDto> CadastrarPedido(PedidoCadastroRequestDto pedidoCadastro);
        Task AtualizarPedido(PedidoAlteracaoRequestDto pedidoAlteracao);
        Task IncluirItemPedido(ItemPedidoRequestDto itemPedido);
        Task ExcluirItemPedido(int idItem, int idPedido);
        Task ExcluirPedido(int idPedido);
        Task<PedidoResponseDto> ObterPedidoPorId(int idPedido);
        Task<IEnumerable<PedidoResponseDto>> ListarPedidos(PedidoFiltroDto pedidoFiltro);
    }
}
