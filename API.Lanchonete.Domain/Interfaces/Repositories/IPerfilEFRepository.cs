using API.Lanchonete.Domain.DTO;
using API.Lanchonete.Domain.Entities;
using API.Lanchonete.Domain.Interfaces.Repositories.Common;

namespace API.Lanchonete.Domain.Interfaces.Repositories
{
    public interface IPerfilEFRepository : IRepositoryBase<Perfil>
    {
        Task<Perfil> CadastrarPerfil(PerfilDto perfil);
    }
}
