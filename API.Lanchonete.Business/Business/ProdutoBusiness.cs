using API.Lanchonete.Domain.DTO.Request;
using API.Lanchonete.Domain.DTO.Request.Filtro;
using API.Lanchonete.Domain.DTO.Response;
using API.Lanchonete.Domain.Entities;
using API.Lanchonete.Domain.Interfaces.Business;
using API.Lanchonete.Domain.Interfaces.Repositories;
using AutoMapper;

namespace API.Lanchonete.Business.Business
{
    public class ProdutoBusiness(IMapper mapper, IProdutoEFRepository produtoEFRepository) : IProdutoBusiness
    {
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly IProdutoEFRepository _produtoRepository = produtoEFRepository ?? throw new ArgumentNullException(nameof(produtoEFRepository));

        public async Task<ProdutoCadastroResponseDto> CadastrarProduto(ProdutoCadastroRequestDto produtoCadastro)
        {
            var produto = _mapper.Map<Produto>(produtoCadastro);
            var result = _mapper.Map<ProdutoCadastroResponseDto>(await _produtoRepository.CadastrarProduto(produto));

            return result;
        }

        public async Task AtualizarProduto(ProdutoAlteracaoRequestDto produtoAlteracao)
        {
            if (produtoAlteracao.IdProduto <= 0)
                throw new ArgumentException("O ID do produto deve ser fornecido para atualização.");

            var produto = _mapper.Map<Produto>(produtoAlteracao);

            await _produtoRepository.AtualizarProduto(produto);
        }

        public async Task ExcluirProduto(int idProduto)
        {
            if (idProduto <= 0)
                throw new ArgumentException("O ID do produto deve ser informado para exclusão.");

            await _produtoRepository.ExcluirProduto(idProduto);
        }

        public async Task<ProdutoCadastroResponseDto> ObterProdutoPorId(int idProduto)
        {
            if (idProduto <= 0)
                throw new ArgumentException("O ID do produto deve ser informado para obter.");

            return _mapper.Map<ProdutoCadastroResponseDto>(await _produtoRepository.ObterProdutoPorId(idProduto));
        }

        public async Task<IEnumerable<ProdutoCadastroResponseDto>> ListarProdutos(ProdutoFiltroDto produtoFiltro)
            => _mapper.Map<IEnumerable<ProdutoCadastroResponseDto>>(await _produtoRepository.ListarProdutos(produtoFiltro));
    }
}
