using API.Lanchonete.Controllers;
using API.Lanchonete.Domain.DTO;
using API.Lanchonete.Domain.DTO.Request.Filtro;
using API.Lanchonete.Domain.Interfaces.Business;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace API.Lanchonete.Tests.Controllers
{
    public class PerfilControllerTest
    {
        private readonly Mock<ILogger<PerfilController>> _loggerPerfilControllerMock;
        private readonly Mock<IPerfilBusiness> _perfilBusinessMock;

        public PerfilControllerTest()
        {
            _loggerPerfilControllerMock = new Mock<ILogger<PerfilController>>();
            _perfilBusinessMock = new Mock<IPerfilBusiness>();
        }

        [Fact]
        public async Task CadastroPerfil_DeveRetornarCreatedResult_QuandoPerfilValido()
        {
            // Arrange
            var perfil = new PerfilDto { Nome = "Cliente", Descricao = "Perfil de cliente" };
            var resultExpected = new PerfilDto { IdPerfil = 1, Nome = "Cliente", Descricao = "Perfil de cliente" };
            _perfilBusinessMock.Setup(s => s.CadastrarPerfil(perfil)).ReturnsAsync(resultExpected);
            var controller = new PerfilController(_loggerPerfilControllerMock.Object, _perfilBusinessMock.Object);

            // Act
            var result = await controller.CadastrarPerfil(perfil);
            var actionResult = Assert.IsType<ActionResult<PerfilDto>>(result);
            var createdResult = Assert.IsType<CreatedResult>(actionResult.Result);
            var perfilResult = Assert.IsType<PerfilDto>(createdResult.Value);

            // Assert
            Assert.Equal(resultExpected, perfilResult);
        }

        [Fact]
        public async Task CadastroPerfil_DeveRetornarBadRequestObjectResult_QuandoRequestInvalido()
        {
            // Arrange
            var perfilRequestInvalido = new PerfilDto { Descricao = string.Empty, Nome = string.Empty };
            var controller = new PerfilController(_loggerPerfilControllerMock.Object, _perfilBusinessMock.Object);
            var mensagensValidacao = new List<ValidationFailure>
            {
                new("Nome", "O nome é obrigatório."),
                new("Nome", "O nome deve ter no mínimo 2 caracteres."),
                new("Descricao", "A descrição é obrigatória.")
            };

            // Act
            var result = await controller.CadastrarPerfil(perfilRequestInvalido);
            var actionResult = Assert.IsType<ActionResult<PerfilDto>>(result);
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            var validationResult = Assert.IsType<List<ValidationFailure>>(badRequestObjectResult.Value);

            // Assert
            Assert.Equal(mensagensValidacao[0].PropertyName, validationResult[0].PropertyName);
            Assert.Equal(mensagensValidacao[0].ErrorMessage, validationResult[0].ErrorMessage);
            Assert.Equal(mensagensValidacao[1].PropertyName, validationResult[1].PropertyName);
            Assert.Equal(mensagensValidacao[1].ErrorMessage, validationResult[1].ErrorMessage);
            Assert.Equal(mensagensValidacao[2].PropertyName, validationResult[2].PropertyName);
            Assert.Equal(mensagensValidacao[2].ErrorMessage, validationResult[2].ErrorMessage);
        }

        [Fact]
        public async Task AtualizarPerfil_DeveRetornarNoContent_QuandoPerfilValido()
        {
            // Arrange
            var perfil = new PerfilDto { IdPerfil = 1, Nome = "Cliente", Descricao = "Perfil atualizado" };
            _perfilBusinessMock.Setup(s => s.AtualizarPerfil(perfil)).Returns(Task.CompletedTask);
            var controller = new PerfilController(_loggerPerfilControllerMock.Object, _perfilBusinessMock.Object);

            // Act
            var result = await controller.AtualizarPerfil(perfil);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task AtualizarPerfil_DeveRetornarBadRequest_QuandoArgumentException()
        {
            // Arrange
            var perfil = new PerfilDto { IdPerfil = 1, Nome = "Cliente", Descricao = "Perfil atualizado" };
            _perfilBusinessMock.Setup(s => s.AtualizarPerfil(perfil)).ThrowsAsync(new ArgumentException("Dados inválidos"));
            var controller = new PerfilController(_loggerPerfilControllerMock.Object, _perfilBusinessMock.Object);

            // Act
            var result = await controller.AtualizarPerfil(perfil);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Dados inválidos", badRequest.Value);
        }

        [Fact]
        public async Task AtualizarPerfil_DeveRetornarNotFound_QuandoKeyNotFoundException()
        {
            // Arrange
            var perfil = new PerfilDto { IdPerfil = 99, Nome = "Inexistente", Descricao = "Perfil inexistente" };
            _perfilBusinessMock.Setup(s => s.AtualizarPerfil(perfil)).ThrowsAsync(new KeyNotFoundException("Perfil não encontrado"));
            var controller = new PerfilController(_loggerPerfilControllerMock.Object, _perfilBusinessMock.Object);

            // Act
            var result = await controller.AtualizarPerfil(perfil);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Perfil não encontrado", notFound.Value);
        }

        [Fact]
        public async Task ExcluirPerfil_DeveRetornarNoContent_QuandoSucesso()
        {
            // Arrange
            int idPerfil = 1;
            _perfilBusinessMock.Setup(s => s.ExcluirPerfil(idPerfil)).Returns(Task.CompletedTask);
            var controller = new PerfilController(_loggerPerfilControllerMock.Object, _perfilBusinessMock.Object);

            // Act
            var result = await controller.ExcluirPerfil(idPerfil);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task ExcluirPerfil_DeveRetornarNotFound_QuandoKeyNotFoundException()
        {
            // Arrange
            int idPerfil = 99;
            _perfilBusinessMock.Setup(s => s.ExcluirPerfil(idPerfil)).ThrowsAsync(new KeyNotFoundException("Perfil não encontrado"));
            var controller = new PerfilController(_loggerPerfilControllerMock.Object, _perfilBusinessMock.Object);

            // Act
            var result = await controller.ExcluirPerfil(idPerfil);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Perfil não encontrado", notFound.Value);
        }

        [Fact]
        public async Task ExcluirPerfil_DeveRetornarBadRequest_QuandoInvalidOperationException()
        {
            // Arrange
            int idPerfil = 2;
            _perfilBusinessMock.Setup(s => s.ExcluirPerfil(idPerfil)).ThrowsAsync(new InvalidOperationException("Perfil associado a usuários"));
            var controller = new PerfilController(_loggerPerfilControllerMock.Object, _perfilBusinessMock.Object);

            // Act
            var result = await controller.ExcluirPerfil(idPerfil);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Perfil associado a usuários", badRequest.Value);
        }

        [Fact]
        public async Task ObterPerfil_DeveRetornarOk_QuandoPerfilExiste()
        {
            // Arrange
            int idPerfil = 1;
            var perfil = new PerfilDto { IdPerfil = idPerfil, Nome = "Cliente", Descricao = "Perfil de cliente" };
            _perfilBusinessMock.Setup(s => s.ObterPerfilPorId(idPerfil)).ReturnsAsync(perfil);
            var controller = new PerfilController(_loggerPerfilControllerMock.Object, _perfilBusinessMock.Object);

            // Act
            var result = await controller.ObterPerfil(idPerfil);

            // Assert
            var actionResult = Assert.IsType<ActionResult<PerfilDto>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var perfilResult = Assert.IsType<PerfilDto>(okResult.Value);
            Assert.Equal(perfil, perfilResult);
        }

        [Fact]
        public async Task ObterPerfil_DeveRetornarNotFound_QuandoPerfilNaoExiste()
        {
            // Arrange
            int idPerfil = 99;
            _perfilBusinessMock.Setup(s => s.ObterPerfilPorId(idPerfil)).ThrowsAsync(new KeyNotFoundException($"Perfil com ID {idPerfil} não encontrado."));
            var controller = new PerfilController(_loggerPerfilControllerMock.Object, _perfilBusinessMock.Object);

            // Act
            var result = await controller.ObterPerfil(idPerfil);

            // Assert
            var actionResult = Assert.IsType<ActionResult<PerfilDto>>(result);
            var notFound = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            Assert.Equal($"Perfil com ID {idPerfil} não encontrado.", notFound.Value);
        }

        [Fact]
        public async Task ListarPerfis_DeveRetornarOk_QuandoPerfisExistem()
        {
            // Arrange
            var perfis = new List<PerfilDto>
                {
                    new() { IdPerfil = 1, Nome = "Cliente", Descricao = "Perfil de cliente" },
                    new() { IdPerfil = 2, Nome = "Admin", Descricao = "Perfil de administrador" }
                };
            _perfilBusinessMock.Setup(s => s.ListarPerfis(It.IsAny<PerfilFiltroDto>())).ReturnsAsync(perfis);
            var controller = new PerfilController(_loggerPerfilControllerMock.Object, _perfilBusinessMock.Object);

            // Act
            var result = await controller.ListarPerfis();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<PerfilDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var perfisResult = Assert.IsAssignableFrom<IEnumerable<PerfilDto>>(okResult.Value);
            Assert.Equal(2, perfisResult.Count());
        }

        [Fact]
        public async Task ListarPerfis_DeveRetornarNotFound_QuandoNenhumPerfilEncontrado()
        {
            // Arrange
            _perfilBusinessMock.Setup(s => s.ListarPerfis(It.IsAny<PerfilFiltroDto>())).ReturnsAsync([]);
            var controller = new PerfilController(_loggerPerfilControllerMock.Object, _perfilBusinessMock.Object);

            // Act
            var result = await controller.ListarPerfis();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<PerfilDto>>>(result);
            var notFound = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            Assert.Equal("Nenhum perfil encontrado.", notFound.Value);
        }

        [Fact]
        public async Task AtualizarPerfil_DeveRetornarBadRequestObjectResult_QuandoRequestInvalido()
        {
            // Arrange
            var perfilRequestInvalido = new PerfilDto { IdPerfil = 1, Nome = string.Empty, Descricao = string.Empty };
            var controller = new PerfilController(_loggerPerfilControllerMock.Object, _perfilBusinessMock.Object);
            var mensagensValidacao = new List<ValidationFailure>
            {
                new("Nome", "O nome é obrigatório."),
                new("Nome", "O nome deve ter no mínimo 2 caracteres."),
                new("Descricao", "A descrição é obrigatória.")
            };

            // Act
            var result = await controller.AtualizarPerfil(perfilRequestInvalido);
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            var validationResult = Assert.IsType<List<ValidationFailure>>(badRequestObjectResult.Value);

            // Assert
            Assert.Equal(mensagensValidacao[0].PropertyName, validationResult[0].PropertyName);
            Assert.Equal(mensagensValidacao[0].ErrorMessage, validationResult[0].ErrorMessage);
            Assert.Equal(mensagensValidacao[1].PropertyName, validationResult[1].PropertyName);
            Assert.Equal(mensagensValidacao[1].ErrorMessage, validationResult[1].ErrorMessage);
            Assert.Equal(mensagensValidacao[2].PropertyName, validationResult[2].PropertyName);
            Assert.Equal(mensagensValidacao[2].ErrorMessage, validationResult[2].ErrorMessage);
        }

        [Fact]
        public async Task ListarPerfis_DeveRetornarBadRequestObjectResult_QuandoFiltroInvalido()
        {
            // Arrange
            var controller = new PerfilController(_loggerPerfilControllerMock.Object, _perfilBusinessMock.Object);
            var filtroInvalido = new PerfilFiltroDto
            {
                Pagina = 0, // Página inválida
                TamanhoPagina = 0 // Tamanho inválido
            };
            var mensagensValidacao = new List<ValidationFailure>
            {
                new("Pagina", "A página deve ser maior que zero."),
                new("TamanhoPagina", "O tamanho da página deve ser maior que zero.")
            };

            // Act
            var result = await controller.ListarPerfis(
                filtroNome: filtroInvalido.Nome,
                filtroDescricao: filtroInvalido.Descricao,
                ordenarPor: filtroInvalido.OrdenarPor,
                ordemDesc: filtroInvalido.OrdemDesc,
                pagina: filtroInvalido.Pagina,
                tamanhoPagina: filtroInvalido.TamanhoPagina
            );
            var actionResult = Assert.IsType<ActionResult<IEnumerable<PerfilDto>>>(result);
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            var validationResult = Assert.IsType<List<ValidationFailure>>(badRequestObjectResult.Value);

            // Assert
            Assert.Equal(mensagensValidacao[0].PropertyName, validationResult[0].PropertyName);
            Assert.Equal(mensagensValidacao[0].ErrorMessage, validationResult[0].ErrorMessage);
            Assert.Equal(mensagensValidacao[1].PropertyName, validationResult[1].PropertyName);
            Assert.Equal(mensagensValidacao[1].ErrorMessage, validationResult[1].ErrorMessage);
        }
    }
}
