using API.Lanchonete.Domain.DTO.Request;
using API.Lanchonete.Domain.DTO.Request.Filtro;
using API.Lanchonete.Domain.DTO.Response;

namespace API.Lanchonete.Domain.Interfaces.Business
{
    public interface IControleAcessoBusiness
    {
        Task<ControleAcessoCadastroResponseDto> CadastrarControleAcesso(ControleAcessoCadastroRequestDto controleAcessoCadastro);
        Task AtualizarControleAcesso(ControleAcessoAlteracaoRequestDto controleAcessoAlteracao);
        Task ExcluirControleAcesso(int idControleAcesso);
        Task<ControleAcessoResponseDto> ObterControleAcessoPorId(int idControleAcesso);
        Task<IEnumerable<ControleAcessoResponseDto>> ListarControleAcessos(ControleAcessoFiltroDto controleAcessoFiltro);
    }
}
