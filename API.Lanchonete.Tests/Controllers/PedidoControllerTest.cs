using API.Lanchonete.Controllers;
using API.Lanchonete.Domain.DTO;
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
    public class PedidoControllerTest
    {
        private readonly Mock<ILogger<PedidoController>> _loggerPedidoControllerMock;
        private readonly Mock<IPedidoBusiness> _pedidoBusinessMock;
        private readonly PedidoController _pedidoController;

        public PedidoControllerTest()
        {
            _loggerPedidoControllerMock = new Mock<ILogger<PedidoController>>();
            _pedidoBusinessMock = new Mock<IPedidoBusiness>();
            _pedidoController = new PedidoController(_loggerPedidoControllerMock.Object, _pedidoBusinessMock.Object);
        }

        #region CadastrarPedido

        [Fact]
        public async Task CadastrarPedido_DeveRetornarSucesso_QuandoCadastroValido()
        {
            // Arrange
            var request = new PedidoCadastroRequestDto
            {
                IdUsuario = 1,
                ItensPedido = [new ItemPedidoCadastroRequestDto { IdProduto = 1, Quantidade = 2 }]
            };
            var response = new PedidoCadastroResponseDto
            {
                IdPedido = 10,
                IdUsuario = 1,
                Status = "Novo",
                ItensPedido = []
            };
            _pedidoBusinessMock.Setup(x => x.CadastrarPedido(It.IsAny<PedidoCadastroRequestDto>()))
                .ReturnsAsync(response);

            var controller = new PedidoController(_loggerPedidoControllerMock.Object, _pedidoBusinessMock.Object);

            // Act
            var result = await controller.CadastrarPedido(request);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result.Result);
            Assert.Equal(response, createdResult.Value);
        }

        [Fact]
        public async Task CadastrarPedido_DeveRetornarFalha_QuandoValidacaoFalhar()
        {
            // Arrange
            var request = new PedidoCadastroRequestDto
            {
                IdUsuario = 0,
                ItensPedido = []
            };
            var controller = new PedidoController(_loggerPedidoControllerMock.Object, _pedidoBusinessMock.Object);

            // Act
            var result = await controller.CadastrarPedido(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task CadastrarPedido_DeveRetornarFalha_QuandoDbUpdateException()
        {
            // Arrange
            var request = new PedidoCadastroRequestDto
            {
                IdUsuario = 1,
                ItensPedido = [new ItemPedidoCadastroRequestDto { IdProduto = 1, Quantidade = 2 }]
            };
            _pedidoBusinessMock.Setup(x => x.CadastrarPedido(It.IsAny<PedidoCadastroRequestDto>()))
                .ThrowsAsync(new Microsoft.EntityFrameworkCore.DbUpdateException("Erro DB", new Exception("Inner")));

            var controller = new PedidoController(_loggerPedidoControllerMock.Object, _pedidoBusinessMock.Object);

            // Act
            var result = await controller.CadastrarPedido(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task CadastrarPedido_DeveRetornarFalha_QuandoException()
        {
            // Arrange
            var request = new PedidoCadastroRequestDto
            {
                IdUsuario = 1,
                ItensPedido = [new ItemPedidoCadastroRequestDto { IdProduto = 1, Quantidade = 2 }]
            };
            _pedidoBusinessMock.Setup(x => x.CadastrarPedido(It.IsAny<PedidoCadastroRequestDto>()))
                .ThrowsAsync(new Exception("Erro genérico"));

            var controller = new PedidoController(_loggerPedidoControllerMock.Object, _pedidoBusinessMock.Object);

            // Act
            var result = await controller.CadastrarPedido(request);

            // Assert
            Assert.IsType<ObjectResult>(result.Result);
        }

        #endregion

        #region AtualizarPedido

        [Fact]
        public async Task AtualizarPedido_DeveRetornarSucesso_QuandoAtualizacaoValida()
        {
            // Arrange
            var request = new PedidoAlteracaoRequestDto
            {
                IdPedido = 1,
                IdUsuario = 1,
                Status = "EmAndamento",
                ItensPedido = [new ItemPedidoAlteracaoRequestDto { IdItem = 1, IdProduto = 1, Quantidade = 2, Status = "pendente" }]
            };
            _pedidoBusinessMock.Setup(x => x.AtualizarPedido(It.IsAny<PedidoAlteracaoRequestDto>()))
                .Returns(Task.CompletedTask);

            var controller = new PedidoController(_loggerPedidoControllerMock.Object, _pedidoBusinessMock.Object);

            // Act
            var result = await controller.AtualizarPedido(request);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task AtualizarPedido_DeveRetornarFalha_QuandoValidacaoFalhar()
        {
            // Arrange
            var request = new PedidoAlteracaoRequestDto
            {
                IdPedido = 0,
                IdUsuario = 0,
                Status = "",
                ItensPedido = []
            };
            var controller = new PedidoController(_loggerPedidoControllerMock.Object, _pedidoBusinessMock.Object);

            // Act
            var result = await controller.AtualizarPedido(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task AtualizarPedido_DeveRetornarFalha_QuandoArgumentException()
        {
            // Arrange
            var request = new PedidoAlteracaoRequestDto
            {
                IdPedido = 1,
                IdUsuario = 1,
                Status = "EmAndamento",
                ItensPedido = []
            };
            _pedidoBusinessMock.Setup(x => x.AtualizarPedido(It.IsAny<PedidoAlteracaoRequestDto>()))
                .ThrowsAsync(new ArgumentException("Argumento inválido"));

            var controller = new PedidoController(_loggerPedidoControllerMock.Object, _pedidoBusinessMock.Object);

            // Act
            var result = await controller.AtualizarPedido(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task AtualizarPedido_DeveRetornarFalha_QuandoDbUpdateException()
        {
            // Arrange
            var request = new PedidoAlteracaoRequestDto
            {
                IdPedido = 1,
                IdUsuario = 1,
                Status = "EmAndamento",
                ItensPedido = []
            };
            _pedidoBusinessMock.Setup(x => x.AtualizarPedido(It.IsAny<PedidoAlteracaoRequestDto>()))
                .ThrowsAsync(new Microsoft.EntityFrameworkCore.DbUpdateException("Erro DB", new Exception("Inner")));

            var controller = new PedidoController(_loggerPedidoControllerMock.Object, _pedidoBusinessMock.Object);

            // Act
            var result = await controller.AtualizarPedido(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task AtualizarPedido_DeveRetornarFalha_QuandoKeyNotFoundException()
        {
            // Arrange
            var request = new PedidoAlteracaoRequestDto
            {
                IdPedido = 1,
                IdUsuario = 1,
                Status = "EmAndamento",
                ItensPedido = [new ItemPedidoAlteracaoRequestDto { IdItem = 1, IdProduto = 1, Quantidade = 2, Status = "pendente" }]
            };
            _pedidoBusinessMock.Setup(x => x.AtualizarPedido(It.IsAny<PedidoAlteracaoRequestDto>()))
                .ThrowsAsync(new KeyNotFoundException("Pedido não encontrado"));

            var controller = new PedidoController(_loggerPedidoControllerMock.Object, _pedidoBusinessMock.Object);

            // Act
            var result = await controller.AtualizarPedido(request);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task AtualizarPedido_DeveRetornarFalha_QuandoException()
        {
            // Arrange
            var request = new PedidoAlteracaoRequestDto
            {
                IdPedido = 1,
                IdUsuario = 1,
                Status = "EmAndamento",
                ItensPedido = [new ItemPedidoAlteracaoRequestDto { IdItem = 1, IdProduto = 1, Quantidade = 2, Status = "pendente" }]
            };
            _pedidoBusinessMock.Setup(x => x.AtualizarPedido(It.IsAny<PedidoAlteracaoRequestDto>()))
                .ThrowsAsync(new Exception("Erro genérico"));

            var controller = new PedidoController(_loggerPedidoControllerMock.Object, _pedidoBusinessMock.Object);

            // Act
            var result = await controller.AtualizarPedido(request);

            // Assert
            Assert.IsType<ObjectResult>(result);
        }

        #endregion

        #region IncluirItemPedido

        [Fact]
        public async Task IncluirItemPedido_DeveRetornarSucesso_QuandoInclusaoValida()
        {
            // Arrange
            int idPedido = 1;
            var item = new ItemPedidoRequestDto { IdProduto = 1, Quantidade = 2 };
            _pedidoBusinessMock.Setup(x => x.IncluirItemPedido(It.IsAny<ItemPedidoRequestDto>()))
                .Returns(Task.CompletedTask);

            var controller = new PedidoController(_loggerPedidoControllerMock.Object, _pedidoBusinessMock.Object);

            // Act
            var result = await controller.IncluirItemPedido(idPedido, item);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task IncluirItemPedido_DeveRetornarFalha_QuandoInvalidOperationException()
        {
            // Arrange
            int idPedido = 1;
            var item = new ItemPedidoRequestDto { IdProduto = 1, Quantidade = 2 };
            _pedidoBusinessMock.Setup(x => x.IncluirItemPedido(It.IsAny<ItemPedidoRequestDto>()))
                .ThrowsAsync(new InvalidOperationException("Operação inválida"));

            var controller = new PedidoController(_loggerPedidoControllerMock.Object, _pedidoBusinessMock.Object);

            // Act
            var result = await controller.IncluirItemPedido(idPedido, item);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task IncluirItemPedido_DeveRetornarFalha_QuandoKeyNotFoundException()
        {
            // Arrange
            int idPedido = 1;
            var item = new ItemPedidoRequestDto { IdProduto = 1, Quantidade = 2 };
            _pedidoBusinessMock.Setup(x => x.IncluirItemPedido(It.IsAny<ItemPedidoRequestDto>()))
                .ThrowsAsync(new KeyNotFoundException("Pedido não encontrado"));

            var controller = new PedidoController(_loggerPedidoControllerMock.Object, _pedidoBusinessMock.Object);

            // Act
            var result = await controller.IncluirItemPedido(idPedido, item);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task IncluirItemPedido_DeveRetornarFalha_QuandoDbUpdateException()
        {
            // Arrange
            int idPedido = 1;
            var item = new ItemPedidoRequestDto { IdProduto = 1, Quantidade = 2 };
            _pedidoBusinessMock.Setup(x => x.IncluirItemPedido(It.IsAny<ItemPedidoRequestDto>()))
                .ThrowsAsync(new Microsoft.EntityFrameworkCore.DbUpdateException("Erro DB", new Exception("Inner")));

            var controller = new PedidoController(_loggerPedidoControllerMock.Object, _pedidoBusinessMock.Object);

            // Act
            var result = await controller.IncluirItemPedido(idPedido, item);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task IncluirItemPedido_DeveRetornarFalha_QuandoException()
        {
            // Arrange
            int idPedido = 1;
            var item = new ItemPedidoRequestDto { IdProduto = 1, Quantidade = 2 };
            _pedidoBusinessMock.Setup(x => x.IncluirItemPedido(It.IsAny<ItemPedidoRequestDto>()))
                .ThrowsAsync(new Exception("Erro genérico"));

            var controller = new PedidoController(_loggerPedidoControllerMock.Object, _pedidoBusinessMock.Object);

            // Act
            var result = await controller.IncluirItemPedido(idPedido, item);

            // Assert
            Assert.IsType<ObjectResult>(result);
        }

        #endregion

        #region ExcluirPedido

        [Fact]
        public async Task ExcluirPedido_DeveRetornarSucesso_QuandoExclusaoValida()
        {
            // Arrange
            int idPedido = 1;
            _pedidoBusinessMock.Setup(x => x.ExcluirPedido(idPedido))
                .Returns(Task.CompletedTask);

            var controller = new PedidoController(_loggerPedidoControllerMock.Object, _pedidoBusinessMock.Object);

            // Act
            var result = await controller.ExcluirPedido(idPedido);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task ExcluirPedido_DeveRetornarFalha_QuandoKeyNotFoundException()
        {
            // Arrange
            int idPedido = 1;
            _pedidoBusinessMock.Setup(x => x.ExcluirPedido(idPedido))
                .ThrowsAsync(new KeyNotFoundException("Pedido não encontrado"));

            var controller = new PedidoController(_loggerPedidoControllerMock.Object, _pedidoBusinessMock.Object);

            // Act
            var result = await controller.ExcluirPedido(idPedido);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task ExcluirPedido_DeveRetornarFalha_QuandoInvalidOperationException()
        {
            // Arrange
            int idPedido = 1;
            _pedidoBusinessMock.Setup(x => x.ExcluirPedido(idPedido))
                .ThrowsAsync(new InvalidOperationException("Associações inválidas"));

            var controller = new PedidoController(_loggerPedidoControllerMock.Object, _pedidoBusinessMock.Object);

            // Act
            var result = await controller.ExcluirPedido(idPedido);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ExcluirPedido_DeveRetornarFalha_QuandoException()
        {
            // Arrange
            int idPedido = 1;
            _pedidoBusinessMock.Setup(x => x.ExcluirPedido(idPedido))
                .ThrowsAsync(new Exception("Erro genérico"));

            var controller = new PedidoController(_loggerPedidoControllerMock.Object, _pedidoBusinessMock.Object);

            // Act
            var result = await controller.ExcluirPedido(idPedido);

            // Assert
            Assert.IsType<ObjectResult>(result);
        }

        #endregion

        #region ExcluirItemPedido

        [Fact]
        public async Task ExcluirItemPedido_DeveRetornarSucesso_QuandoExclusaoValida()
        {
            // Arrange
            int idPedido = 1, idItemPedido = 2;
            _pedidoBusinessMock.Setup(x => x.ExcluirItemPedido(idItemPedido, idPedido))
                .Returns(Task.CompletedTask);

            var controller = new PedidoController(_loggerPedidoControllerMock.Object, _pedidoBusinessMock.Object);

            // Act
            var result = await controller.ExcluirItemPedido(idPedido, idItemPedido);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task ExcluirItemPedido_DeveRetornarFalha_QuandoKeyNotFoundException()
        {
            // Arrange
            int idPedido = 1, idItemPedido = 2;
            _pedidoBusinessMock.Setup(x => x.ExcluirItemPedido(idItemPedido, idPedido))
                .ThrowsAsync(new KeyNotFoundException("Pedido não encontrado"));

            var controller = new PedidoController(_loggerPedidoControllerMock.Object, _pedidoBusinessMock.Object);

            // Act
            var result = await controller.ExcluirItemPedido(idPedido, idItemPedido);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task ExcluirItemPedido_DeveRetornarFalha_QuandoInvalidOperationException()
        {
            // Arrange
            int idPedido = 1, idItemPedido = 2;
            _pedidoBusinessMock.Setup(x => x.ExcluirItemPedido(idItemPedido, idPedido))
                .ThrowsAsync(new InvalidOperationException("Associações inválidas"));

            var controller = new PedidoController(_loggerPedidoControllerMock.Object, _pedidoBusinessMock.Object);

            // Act
            var result = await controller.ExcluirItemPedido(idPedido, idItemPedido);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ExcluirItemPedido_DeveRetornarFalha_QuandoException()
        {
            // Arrange
            int idPedido = 1, idItemPedido = 2;
            _pedidoBusinessMock.Setup(x => x.ExcluirItemPedido(idItemPedido, idPedido))
                .ThrowsAsync(new Exception("Erro genérico"));

            var controller = new PedidoController(_loggerPedidoControllerMock.Object, _pedidoBusinessMock.Object);

            // Act
            var result = await controller.ExcluirItemPedido(idPedido, idItemPedido);

            // Assert
            Assert.IsType<ObjectResult>(result);
        }

        #endregion

        #region ObterPedido

        [Fact]
        public async Task ObterPedido_DeveRetornarSucesso_QuandoPedidoExiste()
        {
            // Arrange
            int idPedido = 1;
            var response = new PedidoResponseDto
            {
                IdPedido = idPedido,
                IdUsuario = 1,
                NomeUsuario = "Teste",
                Status = "Novo",
                DataCadastro = DateTime.Now,
                ItensPedido = []
            };
            _pedidoBusinessMock.Setup(x => x.ObterPedidoPorId(idPedido))
                .ReturnsAsync(response);

            var controller = new PedidoController(_loggerPedidoControllerMock.Object, _pedidoBusinessMock.Object);

            // Act
            var result = await controller.ObterPedido(idPedido);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(response, okResult.Value);
        }

        [Fact]
        public async Task ObterPedido_DeveRetornarFalha_QuandoKeyNotFoundException()
        {
            // Arrange
            int idPedido = 1;
            _pedidoBusinessMock.Setup(x => x.ObterPedidoPorId(idPedido))
                .ThrowsAsync(new KeyNotFoundException("Pedido não encontrado"));

            var controller = new PedidoController(_loggerPedidoControllerMock.Object, _pedidoBusinessMock.Object);

            // Act
            var result = await controller.ObterPedido(idPedido);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task ObterPedido_DeveRetornarFalha_QuandoException()
        {
            // Arrange
            int idPedido = 1;
            _pedidoBusinessMock.Setup(x => x.ObterPedidoPorId(idPedido))
                .ThrowsAsync(new Exception("Erro genérico"));

            var controller = new PedidoController(_loggerPedidoControllerMock.Object, _pedidoBusinessMock.Object);

            // Act
            var result = await controller.ObterPedido(idPedido);

            // Assert
            Assert.IsType<ObjectResult>(result.Result);
        }

        #endregion

        #region ListarPedidos

        [Fact]
        public async Task ListarPedidos_DeveRetornarSucesso_QuandoFiltroValido()
        {
            // Arrange
            IEnumerable<PedidoResponseDto>? response =
                [
                    new() {
                        IdPedido = 1,
                        IdUsuario = 1,
                        NomeUsuario = "Teste",
                        Status = "Novo",
                        DataCadastro = DateTime.Now,
                        ItensPedido = [new ItemPedidoResponseDto { DescricaoProduto = "Dog", IdItem = 1, IdProduto = 1, Quantidade = 2, Status = "pendente" }],
                    }
                ];

            _pedidoBusinessMock.Setup(x => x.ListarPedidos(It.IsAny<PedidoFiltroDto>()))
                .ReturnsAsync(response);

            var controller = new PedidoController(_loggerPedidoControllerMock.Object, _pedidoBusinessMock.Object);

            // Act
            var result = await controller.ListarPedidos();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<PedidoResponseDto>?>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);

            Assert.Equal(response, okObjectResult.Value);
        }

        [Fact]
        public async Task ListarPedidos_DeveRetornarFalha_QuandoValidacaoFalhar()
        {
            // Arrange
            var controller = new PedidoController(_loggerPedidoControllerMock.Object, _pedidoBusinessMock.Object);

            // Act
            var result = await controller.ListarPedidos(pagina: 0);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task ListarPedidos_DeveRetornarFalha_QuandoKeyNotFoundException()
        {
            // Arrange
            _pedidoBusinessMock.Setup(x => x.ListarPedidos(It.IsAny<PedidoFiltroDto>()))
                .ThrowsAsync(new KeyNotFoundException("Nenhum pedido encontrado"));

            var controller = new PedidoController(_loggerPedidoControllerMock.Object, _pedidoBusinessMock.Object);

            // Act
            var result = await controller.ListarPedidos();

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task ListarPedidos_DeveRetornarFalha_QuandoException()
        {
            // Arrange
            _pedidoBusinessMock.Setup(x => x.ListarPedidos(It.IsAny<PedidoFiltroDto>()))
                .ThrowsAsync(new Exception("Erro genérico"));

            var controller = new PedidoController(_loggerPedidoControllerMock.Object, _pedidoBusinessMock.Object);

            // Act
            var result = await controller.ListarPedidos();

            // Assert
            Assert.IsType<ObjectResult>(result.Result);
        }

        #endregion
    }
}
