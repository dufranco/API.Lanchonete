using API.Lanchonete.Controllers;
using API.Lanchonete.Domain.DTO.Request;
using API.Lanchonete.Domain.DTO.Request.Filtro;
using API.Lanchonete.Domain.DTO.Response;
using API.Lanchonete.Domain.Interfaces.Business;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace API.Lanchonete.Tests.Controllers
{
    public class ProdutoControllerTest
    {
        private readonly Mock<ILogger<ProdutoController>> _loggerProdutoControllerMock;
        private readonly Mock<IProdutoBusiness> _produtoBusinessMock;
        private readonly ProdutoController _controller;

        public ProdutoControllerTest()
        {
            _loggerProdutoControllerMock = new Mock<ILogger<ProdutoController>>();
            _produtoBusinessMock = new Mock<IProdutoBusiness>();
            _controller = new ProdutoController(_loggerProdutoControllerMock.Object, _produtoBusinessMock.Object);
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
            var response = new ProdutoCadastroResponseDto
            {
                IdProduto = 1,
                Nome = request.Nome,
                Descricao = request.Descricao,
                Preco = request.Preco,
                Tipo = request.Tipo,
                Ativo = request.Ativo
            };
            _produtoBusinessMock.Setup(x => x.CadastrarProduto(request)).ReturnsAsync(response);

            var result = await _controller.CadastrarProduto(request);

            var createdResult = Assert.IsType<CreatedResult>(result.Result);
            Assert.Equal(response, createdResult.Value);
        }

        [Fact]
        public async Task CadastrarProduto_DeveRetornarFalha_QuandoProdutoInvalido()
        {
            var request = new ProdutoCadastroRequestDto
            {
                Nome = "",
                Descricao = "",
                Preco = 0,
                Tipo = "",
                Ativo = true
            };

            var result = await _controller.CadastrarProduto(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(badRequest.Value);
        }

        [Fact]
        public async Task CadastrarProduto_DeveRetornarFalha_QuandoDbUpdateException()
        {
            var request = new ProdutoCadastroRequestDto
            {
                Nome = "Produto Teste",
                Descricao = "Descrição",
                Preco = 10,
                Tipo = "Tipo",
                Ativo = true
            };
            _produtoBusinessMock.Setup(x => x.CadastrarProduto(request)).ThrowsAsync(new Microsoft.EntityFrameworkCore.DbUpdateException("Erro DB"));

            var result = await _controller.CadastrarProduto(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Erro DB", badRequest.Value);
        }

        [Fact]
        public async Task CadastrarProduto_DeveRetornarFalha_QuandoException()
        {
            var request = new ProdutoCadastroRequestDto
            {
                Nome = "Produto Teste",
                Descricao = "Descrição",
                Preco = 10,
                Tipo = "Tipo",
                Ativo = true
            };
            _produtoBusinessMock.Setup(x => x.CadastrarProduto(request)).ThrowsAsync(new Exception("Erro"));

            var result = await _controller.CadastrarProduto(request);

            var problemResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, problemResult.StatusCode);
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
            _produtoBusinessMock.Setup(x => x.AtualizarProduto(request)).Returns(Task.CompletedTask);

            var result = await _controller.AtualizarProduto(request);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task AtualizarProduto_DeveRetornarFalha_QuandoProdutoInvalido()
        {
            var request = new ProdutoAlteracaoRequestDto
            {
                IdProduto = 0,
                Nome = "",
                Descricao = "",
                Preco = 0,
                Tipo = "",
                Ativo = true
            };

            var result = await _controller.AtualizarProduto(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequest.Value);
        }

        [Fact]
        public async Task AtualizarProduto_DeveRetornarFalha_QuandoArgumentException()
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
            _produtoBusinessMock.Setup(x => x.AtualizarProduto(request)).ThrowsAsync(new ArgumentException("Argumento inválido"));

            var result = await _controller.AtualizarProduto(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Argumento inválido", badRequest.Value);
        }

        [Fact]
        public async Task AtualizarProduto_DeveRetornarFalha_QuandoDbUpdateException()
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
            _produtoBusinessMock.Setup(x => x.AtualizarProduto(request)).ThrowsAsync(new Microsoft.EntityFrameworkCore.DbUpdateException("Erro DB"));

            var result = await _controller.AtualizarProduto(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Erro DB", badRequest.Value);
        }

        [Fact]
        public async Task AtualizarProduto_DeveRetornarFalha_QuandoKeyNotFoundException()
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
            _produtoBusinessMock.Setup(x => x.AtualizarProduto(request)).ThrowsAsync(new KeyNotFoundException("Não encontrado"));

            var result = await _controller.AtualizarProduto(request);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Não encontrado", notFound.Value);
        }

        [Fact]
        public async Task AtualizarProduto_DeveRetornarFalha_QuandoException()
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
            _produtoBusinessMock.Setup(x => x.AtualizarProduto(request)).ThrowsAsync(new Exception("Erro"));

            var result = await _controller.AtualizarProduto(request);

            var problemResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, problemResult.StatusCode);
        }

        [Fact]
        public async Task ExcluirProduto_DeveRetornarSucesso_QuandoProdutoExiste()
        {
            _produtoBusinessMock.Setup(x => x.ExcluirProduto(1)).Returns(Task.CompletedTask);

            var result = await _controller.ExcluirProduto(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task ExcluirProduto_DeveRetornarFalha_QuandoKeyNotFoundException()
        {
            _produtoBusinessMock.Setup(x => x.ExcluirProduto(1)).ThrowsAsync(new KeyNotFoundException("Não encontrado"));

            var result = await _controller.ExcluirProduto(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Não encontrado", notFound.Value);
        }

        [Fact]
        public async Task ExcluirProduto_DeveRetornarFalha_QuandoInvalidOperationException()
        {
            _produtoBusinessMock.Setup(x => x.ExcluirProduto(1)).ThrowsAsync(new InvalidOperationException("Associações"));

            var result = await _controller.ExcluirProduto(1);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Associações", badRequest.Value);
        }

        [Fact]
        public async Task ExcluirProduto_DeveRetornarFalha_QuandoException()
        {
            _produtoBusinessMock.Setup(x => x.ExcluirProduto(1)).ThrowsAsync(new Exception("Erro"));

            var result = await _controller.ExcluirProduto(1);

            var problemResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, problemResult.StatusCode);
        }

        [Fact]
        public async Task ObterProduto_DeveRetornarSucesso_QuandoProdutoExiste()
        {
            var response = new ProdutoCadastroResponseDto
            {
                IdProduto = 1,
                Nome = "Produto Teste",
                Descricao = "Descrição",
                Preco = 10,
                Tipo = "Tipo",
                Ativo = true
            };
            _produtoBusinessMock.Setup(x => x.ObterProdutoPorId(1)).ReturnsAsync(response);

            var result = await _controller.ObterProduto(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(response, okResult.Value);
        }

        [Fact]
        public async Task ObterProduto_DeveRetornarFalha_QuandoKeyNotFoundException()
        {
            _produtoBusinessMock.Setup(x => x.ObterProdutoPorId(1)).ThrowsAsync(new KeyNotFoundException("Não encontrado"));

            var result = await _controller.ObterProduto(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Não encontrado", notFound.Value);
        }

        [Fact]
        public async Task ObterProduto_DeveRetornarFalha_QuandoException()
        {
            _produtoBusinessMock.Setup(x => x.ObterProdutoPorId(1)).ThrowsAsync(new Exception("Erro"));

            var result = await _controller.ObterProduto(1);

            var problemResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, problemResult.StatusCode);
        }

        [Fact]
        public async Task ListarProdutos_DeveRetornarSucesso_QuandoProdutosExistem()
        {
            var filtro = new ProdutoFiltroDto
            {
                IdProduto = null,
                Nome = null,
                Descricao = null,
                Tipo = null,
                Ativo = null,
                OrdenarPor = 0,
                OrdemDesc = false,
                Pagina = 1,
                TamanhoPagina = 10
            };
            var produtos = new List<ProdutoCadastroResponseDto>
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
            _produtoBusinessMock.Setup(x => x.ListarProdutos(It.IsAny<ProdutoFiltroDto>())).ReturnsAsync(produtos);

            var result = await _controller.ListarProdutos();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(produtos, okResult.Value);
        }

        [Fact]
        public async Task ListarProdutos_DeveRetornarFalha_QuandoFiltroInvalido()
        {
            var result = await _controller.ListarProdutos(IdProduto: -1);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(badRequest.Value);
        }

        [Fact]
        public async Task ListarProdutos_DeveRetornarFalha_QuandoNenhumProdutoEncontrado()
        {
            _produtoBusinessMock.Setup(x => x.ListarProdutos(It.IsAny<ProdutoFiltroDto>())).ReturnsAsync(new List<ProdutoCadastroResponseDto>());

            var result = await _controller.ListarProdutos();

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Nenhum produto encontrado.", notFound.Value);
        }

        [Fact]
        public async Task ListarProdutos_DeveRetornarFalha_QuandoKeyNotFoundException()
        {
            _produtoBusinessMock.Setup(x => x.ListarProdutos(It.IsAny<ProdutoFiltroDto>())).ThrowsAsync(new KeyNotFoundException("Não encontrado"));

            var result = await _controller.ListarProdutos();

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Não encontrado", notFound.Value);
        }

        [Fact]
        public async Task ListarProdutos_DeveRetornarFalha_QuandoException()
        {
            _produtoBusinessMock.Setup(x => x.ListarProdutos(It.IsAny<ProdutoFiltroDto>())).ThrowsAsync(new Exception("Erro"));

            var result = await _controller.ListarProdutos();

            var problemResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, problemResult.StatusCode);
        }
    }
}
