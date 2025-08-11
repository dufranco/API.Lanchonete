using API.Lanchonete.Domain.DTO;
using API.Lanchonete.Domain.Entities;
using API.Lanchonete.Domain.Interfaces.Repositories.Common;

namespace API.Lanchonete.Domain.Interfaces.Repositories
{
    public interface IUsuarioEFRepository : IRepositoryBase<Usuario>
    {
        Task<Usuario> CadastrarUsuario(Usuario usuario);
        Task AtualizarUsuario(Usuario usuario);
        Task ExcluirUsuario(int idUsuario);
        Task<Usuario> ObterUsuarioPorId(int idUsuario);
        Task<IEnumerable<Usuario>> ListarUsuarios(UsuarioFiltroDto usuarioFiltro);
    }
}
