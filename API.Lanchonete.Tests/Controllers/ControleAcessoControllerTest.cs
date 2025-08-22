using API.Lanchonete.Controllers;
using API.Lanchonete.Domain.DTO;
using API.Lanchonete.Domain.DTO.Request;
using API.Lanchonete.Domain.DTO.Request.Filtro;
using API.Lanchonete.Domain.DTO.Response;
using API.Lanchonete.Domain.Interfaces.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace API.Lanchonete.Tests.Controllers
{
    public class ControleAcessoControllerTest
    {
        private readonly Mock<ILogger<ControleAcessoController>> _loggerControleAcessoControllerMock;
        private readonly Mock<IControleAcessoBusiness> _controleAcessoBusinessMock;
        private readonly ControleAcessoController _controleAcessoController;

        public ControleAcessoControllerTest()
        {
            _loggerControleAcessoControllerMock = new Mock<ILogger<ControleAcessoController>>();
            _controleAcessoBusinessMock = new Mock<IControleAcessoBusiness>();
            _controleAcessoController = new ControleAcessoController(_loggerControleAcessoControllerMock.Object, _controleAcessoBusinessMock.Object);
        }

        [Fact]
        public async Task CadastrarControleAcesso_DeveRetornarSucesso_QuandoCadastroValido()
        {
            var request = new ControleAcessoCadastroRequestDto { IdPerfil = 1, NomeTela = "Tela1", Permitido = true };
            var response = new ControleAcessoCadastroResponseDto { IdControle = 1, IdPerfil = 1, NomeTela = "Tela1", Permitido = true };

            _controleAcessoBusinessMock.Setup(x => x.CadastrarControleAcesso(It.IsAny<ControleAcessoCadastroRequestDto>()))
                .ReturnsAsync(response);

            var result = await _controleAcessoController.CadastrarControleAcesso(request);

            var createdResult = Assert.IsType<CreatedResult>(result.Result);
            Assert.Equal(response, createdResult.Value);
        }

        [Fact]
        public async Task CadastrarControleAcesso_DeveRetornarFalha_QuandoValidacaoFalhar()
        {
            var request = new ControleAcessoCadastroRequestDto();
            // Força a validação a falhar (NomeTela obrigatório, por exemplo)
            var result = await _controleAcessoController.CadastrarControleAcesso(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(badRequest.Value);
        }

        [Fact]
        public async Task CadastrarControleAcesso_DeveRetornarFalha_QuandoDbUpdateException()
        {
            var request = new ControleAcessoCadastroRequestDto { IdPerfil = 1, NomeTela = "Tela1", Permitido = true };

            _controleAcessoBusinessMock.Setup(x => x.CadastrarControleAcesso(It.IsAny<ControleAcessoCadastroRequestDto>()))
                .ThrowsAsync(new Microsoft.EntityFrameworkCore.DbUpdateException("Erro DB"));

            var result = await _controleAcessoController.CadastrarControleAcesso(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Erro DB", badRequest.Value);
        }

        [Fact]
        public async Task CadastrarControleAcesso_DeveRetornarFalha_QuandoException()
        {
            var request = new ControleAcessoCadastroRequestDto { IdPerfil = 1, NomeTela = "Tela1", Permitido = true };

            _controleAcessoBusinessMock.Setup(x => x.CadastrarControleAcesso(It.IsAny<ControleAcessoCadastroRequestDto>()))
                .ThrowsAsync(new Exception("Erro inesperado"));

            var result = await _controleAcessoController.CadastrarControleAcesso(request);

            var problem = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, problem.StatusCode);
        }

        [Fact]
        public async Task AtualizarControleAcesso_DeveRetornarSucesso_QuandoAtualizacaoValida()
        {
            var request = new ControleAcessoAlteracaoRequestDto { IdControle = 1, IdPerfil = 1, NomeTela = "Tela1", Permitido = true };

            _controleAcessoBusinessMock.Setup(x => x.AtualizarControleAcesso(It.IsAny<ControleAcessoAlteracaoRequestDto>()))
                .Returns(Task.CompletedTask);

            var result = await _controleAcessoController.AtualizarControleAcesso(request);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task AtualizarControleAcesso_DeveRetornarFalha_QuandoValidacaoFalhar()
        {
            var request = new ControleAcessoAlteracaoRequestDto();

            var result = await _controleAcessoController.AtualizarControleAcesso(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequest.Value);
        }

        [Fact]
        public async Task AtualizarControleAcesso_DeveRetornarFalha_QuandoArgumentException()
        {
            var request = new ControleAcessoAlteracaoRequestDto { IdControle = 1, IdPerfil = 1, NomeTela = "Tela1", Permitido = true };

            _controleAcessoBusinessMock.Setup(x => x.AtualizarControleAcesso(It.IsAny<ControleAcessoAlteracaoRequestDto>()))
                .ThrowsAsync(new ArgumentException("Argumento inválido"));

            var result = await _controleAcessoController.AtualizarControleAcesso(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Argumento inválido", badRequest.Value);
        }

        [Fact]
        public async Task AtualizarControleAcesso_DeveRetornarFalha_QuandoDbUpdateException()
        {
            var request = new ControleAcessoAlteracaoRequestDto { IdControle = 1, IdPerfil = 1, NomeTela = "Tela1", Permitido = true };

            _controleAcessoBusinessMock.Setup(x => x.AtualizarControleAcesso(It.IsAny<ControleAcessoAlteracaoRequestDto>()))
                .ThrowsAsync(new Microsoft.EntityFrameworkCore.DbUpdateException("Erro DB"));

            var result = await _controleAcessoController.AtualizarControleAcesso(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Erro DB", badRequest.Value);
        }

        [Fact]
        public async Task AtualizarControleAcesso_DeveRetornarFalha_QuandoKeyNotFoundException()
        {
            var request = new ControleAcessoAlteracaoRequestDto { IdControle = 1, IdPerfil = 1, NomeTela = "Tela1", Permitido = true };

            _controleAcessoBusinessMock.Setup(x => x.AtualizarControleAcesso(It.IsAny<ControleAcessoAlteracaoRequestDto>()))
                .ThrowsAsync(new KeyNotFoundException("Não encontrado"));

            var result = await _controleAcessoController.AtualizarControleAcesso(request);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Não encontrado", notFound.Value);
        }

        [Fact]
        public async Task AtualizarControleAcesso_DeveRetornarFalha_QuandoException()
        {
            var request = new ControleAcessoAlteracaoRequestDto { IdControle = 1, IdPerfil = 1, NomeTela = "Tela1", Permitido = true };

            _controleAcessoBusinessMock.Setup(x => x.AtualizarControleAcesso(It.IsAny<ControleAcessoAlteracaoRequestDto>()))
                .ThrowsAsync(new Exception("Erro inesperado"));

            var result = await _controleAcessoController.AtualizarControleAcesso(request);

            var problem = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, problem.StatusCode);
        }

        [Fact]
        public async Task ExcluirControleAcesso_DeveRetornarSucesso_QuandoExclusaoValida()
        {
            _controleAcessoBusinessMock.Setup(x => x.ExcluirControleAcesso(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            var result = await _controleAcessoController.ExcluirControleAcesso(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task ExcluirControleAcesso_DeveRetornarFalha_QuandoKeyNotFoundException()
        {
            _controleAcessoBusinessMock.Setup(x => x.ExcluirControleAcesso(It.IsAny<int>()))
                .ThrowsAsync(new KeyNotFoundException("Não encontrado"));

            var result = await _controleAcessoController.ExcluirControleAcesso(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Não encontrado", notFound.Value);
        }

        [Fact]
        public async Task ExcluirControleAcesso_DeveRetornarFalha_QuandoInvalidOperationException()
        {
            _controleAcessoBusinessMock.Setup(x => x.ExcluirControleAcesso(It.IsAny<int>()))
                .ThrowsAsync(new InvalidOperationException("Associações existentes"));

            var result = await _controleAcessoController.ExcluirControleAcesso(1);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Associações existentes", badRequest.Value);
        }

        [Fact]
        public async Task ExcluirControleAcesso_DeveRetornarFalha_QuandoException()
        {
            _controleAcessoBusinessMock.Setup(x => x.ExcluirControleAcesso(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Erro inesperado"));

            var result = await _controleAcessoController.ExcluirControleAcesso(1);

            var problem = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, problem.StatusCode);
        }

        [Fact]
        public async Task ObterControleAcesso_DeveRetornarSucesso_QuandoEncontrado()
        {
            var response = new ControleAcessoResponseDto { IdControle = 1, IdPerfil = 1, NomeTela = "Tela1", Permitido = true, DataCadastro = DateTime.Now, NomePerfil = "Perfil" };

            _controleAcessoBusinessMock.Setup(x => x.ObterControleAcessoPorId(It.IsAny<int>()))
                .ReturnsAsync(response);

            var result = await _controleAcessoController.ObterControleAcesso(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(response, okResult.Value);
        }

        [Fact]
        public async Task ObterControleAcesso_DeveRetornarFalha_QuandoKeyNotFoundException()
        {
            _controleAcessoBusinessMock.Setup(x => x.ObterControleAcessoPorId(It.IsAny<int>()))
                .ThrowsAsync(new KeyNotFoundException("Não encontrado"));

            var result = await _controleAcessoController.ObterControleAcesso(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Não encontrado", notFound.Value);
        }

        [Fact]
        public async Task ObterControleAcesso_DeveRetornarFalha_QuandoException()
        {
            _controleAcessoBusinessMock.Setup(x => x.ObterControleAcessoPorId(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Erro inesperado"));

            var result = await _controleAcessoController.ObterControleAcesso(1);

            var problem = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, problem.StatusCode);
        }

        [Fact]
        public async Task ListarControleAcessos_DeveRetornarSucesso_QuandoEncontrarRegistros()
        {
            var response = new List<ControleAcessoResponseDto>
                {
                    new ControleAcessoResponseDto { IdControle = 1, IdPerfil = 1, NomeTela = "Tela1", Permitido = true, DataCadastro = DateTime.Now, NomePerfil = "Perfil" }
                };

            _controleAcessoBusinessMock.Setup(x => x.ListarControleAcessos(It.IsAny<ControleAcessoFiltroDto>()))
                .ReturnsAsync(response);

            var result = await _controleAcessoController.ListarControleAcessos();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(response, okResult.Value);
        }

        [Fact]
        public async Task ListarControleAcessos_DeveRetornarFalha_QuandoValidacaoFalhar()
        {
            // Força a validação a falhar (ex: pagina negativa)
            var result = await _controleAcessoController.ListarControleAcessos(Pagina: -1);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(badRequest.Value);
        }

        [Fact]
        public async Task ListarControleAcessos_DeveRetornarFalha_QuandoNaoEncontrarRegistros()
        {
            _controleAcessoBusinessMock.Setup(x => x.ListarControleAcessos(It.IsAny<ControleAcessoFiltroDto>()))
                .ReturnsAsync(new List<ControleAcessoResponseDto>());

            var result = await _controleAcessoController.ListarControleAcessos();

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Nenhum controle de acesso encontrado.", notFound.Value);
        }

        [Fact]
        public async Task ListarControleAcessos_DeveRetornarFalha_QuandoKeyNotFoundException()
        {
            _controleAcessoBusinessMock.Setup(x => x.ListarControleAcessos(It.IsAny<ControleAcessoFiltroDto>()))
                .ThrowsAsync(new KeyNotFoundException("Não encontrado"));

            var result = await _controleAcessoController.ListarControleAcessos();

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Não encontrado", notFound.Value);
        }

        [Fact]
        public async Task ListarControleAcessos_DeveRetornarFalha_QuandoException()
        {
            _controleAcessoBusinessMock.Setup(x => x.ListarControleAcessos(It.IsAny<ControleAcessoFiltroDto>()))
                .ThrowsAsync(new Exception("Erro inesperado"));

            var result = await _controleAcessoController.ListarControleAcessos();

            var problem = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, problem.StatusCode);
        }
    }
}
