using API.Lanchonete.Business.Business;
using API.Lanchonete.Domain.DTO;
using API.Lanchonete.Domain.DTO.Request;
using API.Lanchonete.Domain.DTO.Request.Filtro;
using API.Lanchonete.Domain.DTO.Response;
using API.Lanchonete.Domain.Entities;
using API.Lanchonete.Domain.Interfaces.Repositories;
using AutoMapper;
using Moq;

namespace API.Lanchonete.Tests.Business
{
    public class ProdutoBusinessTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IProdutoEFRepository> _produtoRepoMock;
        private readonly ProdutoBusiness _produtoBusiness;

        public ProdutoBusinessTest()
        {
            _mapperMock = new Mock<IMapper>();
            _produtoRepoMock = new Mock<IProdutoEFRepository>();
            _produtoBusiness = new ProdutoBusiness(_mapperMock.Object, _produtoRepoMock.Object);
        }

        [Fact]
        public async Task CadastrarProduto_DeveRetornarSucesso_QuandoProdutoValido()
        {
            var request = new ProdutoCadastroRequestDto
            {
                Nome = "Produto Teste",
                Descricao = "Descrição",
                Preco = 10,
                Tipo = "Tipo",
                Ativo = true
            };
            var produto = new Produto
            {
                IdProduto = 1,
                Nome = request.Nome,
                Descricao = request.Descricao,
                Preco = request.Preco,
                Tipo = request.Tipo,
                Ativo = request.Ativo
            };
            var response = new ProdutoCadastroResponseDto
            {
                IdProduto = produto.IdProduto,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                Preco = produto.Preco,
                Tipo = produto.Tipo,
                Ativo = produto.Ativo ?? true
            };

            _mapperMock.Setup(m => m.Map<Produto>(request)).Returns(produto);
            _produtoRepoMock.Setup(r => r.CadastrarProduto(produto)).ReturnsAsync(produto);
            _mapperMock.Setup(m => m.Map<ProdutoCadastroResponseDto>(produto)).Returns(response);

            var result = await _produtoBusiness.CadastrarProduto(request);

            Assert.NotNull(result);
            Assert.Equal(response.IdProduto, result.IdProduto);
        }

        [Fact]
        public async Task AtualizarProduto_DeveRetornarSucesso_QuandoProdutoValido()
        {
            var request = new ProdutoAlteracaoRequestDto
            {
                IdProduto = 1,
                Nome = "Produto Teste",
                Descricao = "Descrição",
                Preco = 10,
                Tipo = "Tipo",
                Ativo = true
            };
            var produto = new Produto
            {
                IdProduto = request.IdProduto,
                Nome = request.Nome,
                Descricao = request.Descricao,
                Preco = request.Preco,
                Tipo = request.Tipo,
                Ativo = request.Ativo
            };

            _mapperMock.Setup(m => m.Map<Produto>(request)).Returns(produto);
            _produtoRepoMock.Setup(r => r.AtualizarProduto(produto)).Returns(Task.CompletedTask);

            await _produtoBusiness.AtualizarProduto(request);

            _produtoRepoMock.Verify(r => r.AtualizarProduto(produto), Times.Once);
        }

        [Fact]
        public async Task AtualizarProduto_DeveRetornarFalha_QuandoIdProdutoInvalido()
        {
            var request = new ProdutoAlteracaoRequestDto
            {
                IdProduto = 0,
                Nome = "Produto Teste",
                Descricao = "Descrição",
                Preco = 10,
                Tipo = "Tipo",
                Ativo = true
            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _produtoBusiness.AtualizarProduto(request));
            Assert.Contains("O ID do produto deve ser fornecido para atualização.", ex.Message);
        }

        [Fact]
        public async Task ExcluirProduto_DeveRetornarSucesso_QuandoIdProdutoValido()
        {
            int idProduto = 1;
            _produtoRepoMock.Setup(r => r.ExcluirProduto(idProduto)).Returns(Task.CompletedTask);

            await _produtoBusiness.ExcluirProduto(idProduto);

            _produtoRepoMock.Verify(r => r.ExcluirProduto(idProduto), Times.Once);
        }

        [Fact]
        public async Task ExcluirProduto_DeveRetornarFalha_QuandoIdProdutoInvalido()
        {
            int idProduto = 0;

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _produtoBusiness.ExcluirProduto(idProduto));
            Assert.Contains("O ID do produto deve ser informado para exclusão.", ex.Message);
        }

        [Fact]
        public async Task ObterProdutoPorId_DeveRetornarSucesso_QuandoIdProdutoValido()
        {
            int idProduto = 1;
            var produto = new Produto
            {
                IdProduto = idProduto,
                Nome = "Produto Teste",
                Descricao = "Descrição",
                Preco = 10,
                Tipo = "Tipo",
                Ativo = true
            };
            var response = new ProdutoCadastroResponseDto
            {
                IdProduto = produto.IdProduto,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                Preco = produto.Preco,
                Tipo = produto.Tipo,
                Ativo = produto.Ativo ?? true
            };

            _produtoRepoMock.Setup(r => r.ObterProdutoPorId(idProduto)).ReturnsAsync(produto);
            _mapperMock.Setup(m => m.Map<ProdutoCadastroResponseDto>(produto)).Returns(response);

            var result = await _produtoBusiness.ObterProdutoPorId(idProduto);

            Assert.NotNull(result);
            Assert.Equal(response.IdProduto, result.IdProduto);
        }

        [Fact]
        public async Task ObterProdutoPorId_DeveRetornarFalha_QuandoIdProdutoInvalido()
        {
            int idProduto = 0;

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _produtoBusiness.ObterProdutoPorId(idProduto));
            Assert.Contains("O ID do produto deve ser informado para obter.", ex.Message);
        }

        [Fact]
        public async Task ListarProdutos_DeveRetornarSucesso_QuandoFiltroValido()
        {
            var filtro = new ProdutoFiltroDto
            {
                Nome = "Teste"
            };
            var produtos = new List<Produto>
                {
                    new Produto
                    {
                        IdProduto = 1,
                        Nome = "Produto Teste",
                        Descricao = "Descrição",
                        Preco = 10,
                        Tipo = "Tipo",
                        Ativo = true
                    }
                };
            var responses = new List<ProdutoCadastroResponseDto>
                {
                    new ProdutoCadastroResponseDto
                    {
                        IdProduto = 1,
                        Nome = "Produto Teste",
                        Descricao = "Descrição",
                        Preco = 10,
                        Tipo = "Tipo",
                        Ativo = true
                    }
                };

            _produtoRepoMock.Setup(r => r.ListarProdutos(filtro)).ReturnsAsync(produtos);
            _mapperMock.Setup(m => m.Map<IEnumerable<ProdutoCadastroResponseDto>>(produtos)).Returns(responses);

            var result = await _produtoBusiness.ListarProdutos(filtro);

            Assert.NotNull(result);
            Assert.Single(result);
        }
    }
}
