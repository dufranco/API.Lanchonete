using API.Lanchonete.Controllers;
using API.Lanchonete.Domain.DTO;
using API.Lanchonete.Domain.Entities;
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
            var resultExpected = new Perfil { IdPerfil = 1, Nome = "Cliente", Descricao = "Perfil de cliente" };
            _perfilBusinessMock.Setup(s => s.CadastrarPerfil(perfil)).ReturnsAsync(resultExpected);
            var controller = new PerfilController(_loggerPerfilControllerMock.Object, _perfilBusinessMock.Object);

            // Act
            var result = await controller.CadastroPerfil(perfil);
            var actionResult = Assert.IsType<ActionResult<Perfil>>(result);
            var createdResult = Assert.IsType<CreatedResult>(actionResult.Result);
            var perfilResult = Assert.IsType<Perfil>(createdResult.Value);

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
            var result = await controller.CadastroPerfil(perfilRequestInvalido);
            var actionResult = Assert.IsType<ActionResult<Perfil>>(result);
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
    }
}
