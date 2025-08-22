using API.Lanchonete.Domain.DTO.Request;
using API.Lanchonete.Domain.DTO.Request.Filtro;
using API.Lanchonete.Domain.DTO.Response;
using API.Lanchonete.Domain.Entities;
using API.Lanchonete.Domain.Interfaces.Business;
using API.Lanchonete.Domain.Interfaces.Repositories;
using AutoMapper;

namespace API.Lanchonete.Business.Business
{
    public class ControleAcessoBusiness(IMapper mapper, IControleAcessoEFRepository controleAcessoEFRepository) : IControleAcessoBusiness
    {
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly IControleAcessoEFRepository _controleAcessoRepository = controleAcessoEFRepository ?? throw new ArgumentNullException(nameof(controleAcessoEFRepository));

        public async Task<ControleAcessoCadastroResponseDto> CadastrarControleAcesso(ControleAcessoCadastroRequestDto controleAcessoCadastro)
        {
            var controleAcesso = _mapper.Map<ControleAcesso>(controleAcessoCadastro);
            var result = _mapper.Map<ControleAcessoCadastroResponseDto>(await _controleAcessoRepository.CadastrarControleAcesso(controleAcesso));

            return result;
        }

        public async Task AtualizarControleAcesso(ControleAcessoAlteracaoRequestDto controleAcessoAlteracao)
        {
            if (controleAcessoAlteracao.IdControle <= 0)
                throw new ArgumentException("O ID do controle de acesso deve ser fornecido para atualização.");

            var controleAcesso = _mapper.Map<ControleAcesso>(controleAcessoAlteracao);

            await _controleAcessoRepository.AtualizarControleAcesso(controleAcesso);
        }

        public async Task ExcluirControleAcesso(int idControleAcesso)
        {
            if (idControleAcesso <= 0)
                throw new ArgumentException("O ID do controle de acesso deve ser informado para exclusão.");

            await _controleAcessoRepository.ExcluirControleAcesso(idControleAcesso);
        }

        public async Task<ControleAcessoResponseDto> ObterControleAcessoPorId(int idControleAcesso)
        {
            if (idControleAcesso <= 0)
                throw new ArgumentException("O ID do controle de acesso deve ser informado para obter.");

            return _mapper.Map<ControleAcessoResponseDto>(await _controleAcessoRepository.ObterControleAcessoPorId(idControleAcesso));
        }

        public async Task<IEnumerable<ControleAcessoResponseDto>> ListarControleAcessos(ControleAcessoFiltroDto controleAcessoFiltro)
            => _mapper.Map<IEnumerable<ControleAcessoResponseDto>>(await _controleAcessoRepository.ListarControleAcessos(controleAcessoFiltro));
    }
}
