using API.Lanchonete.Domain.DTO;
using API.Lanchonete.Domain.Entities;

namespace API.Lanchonete.Domain.Interfaces.Business
{
    public interface IPerfilBusiness
    {
        Task<Perfil> CadastrarPerfil(PerfilDto perfil);
    }
}
