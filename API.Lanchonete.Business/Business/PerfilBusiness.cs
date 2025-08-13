using API.Lanchonete.Domain.DTO;
using API.Lanchonete.Domain.DTO.Request.Filtro;
using API.Lanchonete.Domain.Interfaces.Business;
using API.Lanchonete.Domain.Interfaces.Repositories;
using AutoMapper;

namespace API.Lanchonete.Business.Business
{
    public class PerfilBusiness(IMapper mapper, IPerfilEFRepository perfilEFRepository) : IPerfilBusiness
    {
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly IPerfilEFRepository _perfilRepository = perfilEFRepository ?? throw new ArgumentNullException(nameof(perfilEFRepository));

        public async Task<PerfilDto> CadastrarPerfil(PerfilDto perfil)
            => _mapper.Map<PerfilDto>(await _perfilRepository.CadastrarPerfil(perfil));

        public async Task AtualizarPerfil(PerfilDto perfil)
        {
            if (!perfil.IdPerfil.HasValue)
                throw new ArgumentException("O ID do perfil deve ser fornecido para atualização.");

            await _perfilRepository.AtualizarPerfil(perfil);
        }

        public async Task ExcluirPerfil(int idPerfil)
        {
            if (idPerfil <= 0)
                throw new ArgumentException("O ID do perfil deve ser informado para exclusão.");

            await _perfilRepository.ExcluirPerfil(idPerfil);
        }

        public async Task<PerfilDto> ObterPerfilPorId(int idPerfil)
        {
            if (idPerfil <= 0)
                throw new ArgumentException("O ID do perfil deve ser informado para obter.");

            return _mapper.Map<PerfilDto>(await _perfilRepository.ObterPerfilPorId(idPerfil));
        }

        public async Task<IEnumerable<PerfilDto>> ListarPerfis(PerfilFiltroDto perfilFiltro)
            => _mapper.Map<IEnumerable<PerfilDto>>(await _perfilRepository.ListarPerfis(perfilFiltro));
    }
}
