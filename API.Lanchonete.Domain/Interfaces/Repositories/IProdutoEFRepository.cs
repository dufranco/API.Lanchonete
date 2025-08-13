using API.Lanchonete.Domain.DTO.Request.Filtro;
using API.Lanchonete.Domain.Entities;
using API.Lanchonete.Domain.Interfaces.Repositories.Common;

namespace API.Lanchonete.Domain.Interfaces.Repositories
{
    public interface IProdutoEFRepository : IRepositoryBase<Produto>
    {
        Task<Produto> CadastrarProduto(Produto produto);
        Task AtualizarProduto(Produto produto);
        Task ExcluirProduto(int idProduto);
        Task<Produto> ObterProdutoPorId(int idProduto);
        Task<IEnumerable<Produto>> ListarProdutos(ProdutoFiltroDto produtoFiltro);
    }
}
