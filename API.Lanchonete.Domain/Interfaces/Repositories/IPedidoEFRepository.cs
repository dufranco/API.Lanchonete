using API.Lanchonete.Domain.DTO.Request;
using API.Lanchonete.Domain.DTO.Request.Filtro;
using API.Lanchonete.Domain.Entities;
using API.Lanchonete.Domain.Interfaces.Repositories.Common;

namespace API.Lanchonete.Domain.Interfaces.Repositories
{
    public interface IPedidoEFRepository : IRepositoryBase<Pedido>
    {
        Task<Pedido> CadastrarPedido(Pedido pedidoCadastro);
        Task AtualizarPedido(Pedido pedidoAlteracao);
        Task ExcluirPedido(int idPedido);
        Task<IEnumerable<Pedido>> ListarPedidos(PedidoFiltroDto pedidoFiltro);
        Task<Pedido> ObterPedidoPorId(int idPedido);
    }
}
