using API.Lanchonete.Domain.DTO;
using API.Lanchonete.Domain.Entities;
using API.Lanchonete.Domain.Interfaces.Business;
using API.Lanchonete.Domain.Interfaces.Repositories;

namespace API.Lanchonete.Business.Business
{
    public class PerfilBusiness : IPerfilBusiness
    {
        private readonly IPerfilEFRepository _perfilRepository;

        public PerfilBusiness(IPerfilEFRepository perfilEFRepository)
        {
            _perfilRepository = perfilEFRepository ?? throw new ArgumentNullException(nameof(perfilEFRepository));
        }

        public async Task<Perfil> CadastrarPerfil(PerfilDto perfil)
            => await _perfilRepository.CadastrarPerfil(perfil);
    }
}
