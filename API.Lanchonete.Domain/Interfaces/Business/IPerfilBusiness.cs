using API.Lanchonete.Domain.DTO;
using API.Lanchonete.Domain.DTO.Request.Filtro;

namespace API.Lanchonete.Domain.Interfaces.Business
{
    public interface IPerfilBusiness
    {
        Task<PerfilDto> CadastrarPerfil(PerfilDto perfil);
        Task AtualizarPerfil(PerfilDto perfil);
        Task ExcluirPerfil(int idPerfil);
        Task<PerfilDto> ObterPerfilPorId(int idPerfil);
        Task<IEnumerable<PerfilDto>> ListarPerfis(PerfilFiltroDto perfilFiltro);
    }
}
