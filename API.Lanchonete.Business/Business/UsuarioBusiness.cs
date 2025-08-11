using API.Lanchonete.Domain.DTO;
using API.Lanchonete.Domain.DTO.Request;
using API.Lanchonete.Domain.DTO.Response;
using API.Lanchonete.Domain.Entities;
using API.Lanchonete.Domain.Interfaces.Business;
using API.Lanchonete.Domain.Interfaces.Repositories;
using AutoMapper;

namespace API.Lanchonete.Business.Business
{
    public class UsuarioBusiness(IMapper mapper, IUsuarioEFRepository usuarioEFRepository) : IUsuarioBusiness
    {
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly IUsuarioEFRepository _usuarioRepository = usuarioEFRepository ?? throw new ArgumentNullException(nameof(usuarioEFRepository));

        public async Task<UsuarioCadastroResponseDto> CadastrarUsuario(UsuarioCadastroRequestDto usuarioCadastro)
        {
            var usuario = _mapper.Map<Usuario>(usuarioCadastro);
            var result = _mapper.Map<UsuarioCadastroResponseDto>(await _usuarioRepository.CadastrarUsuario(usuario));

            return result;
        }

        public async Task AtualizarUsuario(UsuarioAlteracaoRequestDto usuarioAlteracao)
        {
            if (usuarioAlteracao.IdUsuario <= 0)
                throw new ArgumentException("O ID do usuário deve ser fornecido para atualização.");

            var usuario = _mapper.Map<Usuario>(usuarioAlteracao);

            await _usuarioRepository.AtualizarUsuario(usuario);
        }

        public async Task ExcluirUsuario(int idUsuario)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("O ID do usuário deve ser informado para exclusão.");

            await _usuarioRepository.ExcluirUsuario(idUsuario);
        }

        public async Task<UsuarioCadastroResponseDto> ObterUsuarioPorId(int idUsuario)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("O ID do usuário deve ser informado para obter.");

            return _mapper.Map<UsuarioCadastroResponseDto>(await _usuarioRepository.ObterUsuarioPorId(idUsuario));
        }

        public async Task<IEnumerable<UsuarioCadastroResponseDto>> ListarUsuarios(UsuarioFiltroDto usuarioFiltro)
            => _mapper.Map<IEnumerable<UsuarioCadastroResponseDto>>(await _usuarioRepository.ListarUsuarios(usuarioFiltro));
    }
}
