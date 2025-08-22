using API.Lanchonete.Domain.DTO.Request.Filtro;
using API.Lanchonete.Domain.Entities;
using API.Lanchonete.Domain.Interfaces.Repositories.Common;

namespace API.Lanchonete.Domain.Interfaces.Repositories
{
    public interface IControleAcessoEFRepository : IRepositoryBase<ControleAcesso>
    {
        Task<ControleAcesso> CadastrarControleAcesso(ControleAcesso controleAcessoCadastro);
        Task AtualizarControleAcesso(ControleAcesso controleAcessoAlteracao);
        Task ExcluirControleAcesso(int idControleAcesso);
        Task<IEnumerable<ControleAcesso>> ListarControleAcessos(ControleAcessoFiltroDto controleAcessoFiltro);
        Task<ControleAcesso> ObterControleAcessoPorId(int idControleAcesso);
    }
}
