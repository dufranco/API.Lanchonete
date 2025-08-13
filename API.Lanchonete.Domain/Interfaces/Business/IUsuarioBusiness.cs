using API.Lanchonete.Domain.DTO.Request;
using API.Lanchonete.Domain.DTO.Request.Filtro;
using API.Lanchonete.Domain.DTO.Response;

namespace API.Lanchonete.Domain.Interfaces.Business
{
    public interface IUsuarioBusiness
    {
        Task<UsuarioCadastroResponseDto> CadastrarUsuario(UsuarioCadastroRequestDto usuario);
        Task AtualizarUsuario(UsuarioAlteracaoRequestDto usuario);
        Task ExcluirUsuario(int idUsuario);
        Task<UsuarioCadastroResponseDto> ObterUsuarioPorId(int idUsuario);
        Task<IEnumerable<UsuarioCadastroResponseDto>> ListarUsuarios(UsuarioFiltroDto usuarioFiltro);
    }
}
