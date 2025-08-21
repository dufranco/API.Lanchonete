using API.Lanchonete.Domain.DTO.Request;
using API.Lanchonete.Domain.Entities;
using API.Lanchonete.Domain.Interfaces.Repositories.Common;

namespace API.Lanchonete.Domain.Interfaces.Repositories
{
    public interface IItensPedidoEFRepository : IRepositoryBase<ItensPedido>
    {
        Task<IEnumerable<ItensPedido>> CadastrarItensPedido(List<ItensPedido> itensPedido);
        Task AtualizarItensPedido(List<ItensPedido> itensPedido);
        Task IncluirItemPedido(ItensPedido itemPedido);
        Task ExcluirItemPedido(int idItem, int idPedido);
        Task ExcluirItensPedido(int idPedido);
    }
}
