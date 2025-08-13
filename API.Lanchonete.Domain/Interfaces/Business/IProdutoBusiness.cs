using API.Lanchonete.Domain.DTO.Request;
using API.Lanchonete.Domain.DTO.Request.Filtro;
using API.Lanchonete.Domain.DTO.Response;

namespace API.Lanchonete.Domain.Interfaces.Business
{
    public interface IProdutoBusiness
    {
        Task<ProdutoCadastroResponseDto> CadastrarProduto(ProdutoCadastroRequestDto perfil);
        Task AtualizarProduto(ProdutoAlteracaoRequestDto perfil);
        Task ExcluirProduto(int idProduto);
        Task<ProdutoCadastroResponseDto> ObterProdutoPorId(int idProduto);
        Task<IEnumerable<ProdutoCadastroResponseDto>> ListarProdutos(ProdutoFiltroDto produtoFiltro);
    }
}
