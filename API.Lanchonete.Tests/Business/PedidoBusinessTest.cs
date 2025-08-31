using API.Lanchonete.Business.Business;
using API.Lanchonete.Domain.DTO.Request;
using API.Lanchonete.Domain.DTO.Request.Filtro;
using API.Lanchonete.Domain.DTO.Response;
using API.Lanchonete.Domain.Entities;
using API.Lanchonete.Domain.Interfaces.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;

namespace API.Lanchonete.Tests.Business
{
    public class PedidoBusinessTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<PedidoBusiness>> _loggerMock;
        private readonly Mock<IPedidoEFRepository> _pedidoRepoMock;
        private readonly Mock<IItensPedidoEFRepository> _itensPedidoRepoMock;
        private readonly Mock<IDbContextTransaction> _transactionMock;
        private readonly PedidoBusiness _pedidoBusiness;

        public PedidoBusinessTest()
        {
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<PedidoBusiness>>();
            _pedidoRepoMock = new Mock<IPedidoEFRepository>();
            _itensPedidoRepoMock = new Mock<IItensPedidoEFRepository>();
            _transactionMock = new Mock<IDbContextTransaction>();
            _pedidoBusiness = new PedidoBusiness(_loggerMock.Object, _mapperMock.Object, _pedidoRepoMock.Object, _itensPedidoRepoMock.Object);
        }

        [Fact]
        public async Task CadastrarPedido_DeveRetornarSucesso_QuandoCadastroForValido()
        {
            // Arrange
            var request = new PedidoCadastroRequestDto
            {
                IdUsuario = 1,
                ItensPedido = new List<ItemPedidoCadastroRequestDto>
                    {
                        new() { IdProduto = 1, Quantidade = 2 }
                    }
            };
            var pedido = new Pedido { IdPedido = 10, IdUsuario = 1, Status = "Novo" };
            var itensPedido = new List<ItensPedido>
                {
                    new() { IdItem = 1, IdPedido = 10, IdProduto = 1, Quantidade = 2, Status = "Novo" }
                };

            _pedidoRepoMock.Setup(x => x.BeginTransactionAsync()).ReturnsAsync(_transactionMock.Object);
            _mapperMock.Setup(x => x.Map<Pedido>(request)).Returns(pedido);
            _pedidoRepoMock.Setup(x => x.CadastrarPedido(It.IsAny<Pedido>())).ReturnsAsync(pedido);
            _itensPedidoRepoMock.Setup(x => x.CadastrarItensPedido(It.IsAny<List<ItensPedido>>())).ReturnsAsync(itensPedido);

            // Act
            var result = await _pedidoBusiness.CadastrarPedido(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.IdPedido);
            Assert.Single(result.ItensPedido);
            _pedidoRepoMock.Verify(x => x.BeginTransactionAsync(), Times.Once);
            _pedidoRepoMock.Verify(x => x.CadastrarPedido(It.IsAny<Pedido>()), Times.Once);
            _itensPedidoRepoMock.Verify(x => x.CadastrarItensPedido(It.IsAny<List<ItensPedido>>()), Times.Once);
            _transactionMock.Verify(x => x.CommitAsync(default), Times.Once);
        }

        [Fact]
        public async Task CadastrarPedido_DeveRetornarFalha_QuandoDbUpdateExceptionForLancada()
        {
            // Arrange
            var request = new PedidoCadastroRequestDto
            {
                IdUsuario = 1,
                ItensPedido = new List<ItemPedidoCadastroRequestDto>
                    {
                        new() { IdProduto = 1, Quantidade = 2 }
                    }
            };
            var pedido = new Pedido { IdPedido = 10, IdUsuario = 1, Status = "Novo" };

            _pedidoRepoMock.Setup(x => x.BeginTransactionAsync()).ReturnsAsync(_transactionMock.Object);
            _mapperMock.Setup(x => x.Map<Pedido>(request)).Returns(pedido);
            _pedidoRepoMock.Setup(x => x.CadastrarPedido(It.IsAny<Pedido>())).ThrowsAsync(new DbUpdateException());

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(() => _pedidoBusiness.CadastrarPedido(request));
            _transactionMock.Verify(x => x.RollbackAsync(default), Times.Once);
        }

        [Fact]
        public async Task CadastrarPedido_DeveRetornarFalha_QuandoExceptionForLancada()
        {
            // Arrange
            var request = new PedidoCadastroRequestDto
            {
                IdUsuario = 1,
                ItensPedido = new List<ItemPedidoCadastroRequestDto>
                    {
                        new() { IdProduto = 1, Quantidade = 2 }
                    }
            };
            var pedido = new Pedido { IdPedido = 10, IdUsuario = 1, Status = "Novo" };

            _pedidoRepoMock.Setup(x => x.BeginTransactionAsync()).ReturnsAsync(_transactionMock.Object);
            _mapperMock.Setup(x => x.Map<Pedido>(request)).Returns(pedido);
            _pedidoRepoMock.Setup(x => x.CadastrarPedido(It.IsAny<Pedido>())).ThrowsAsync(new Exception("Erro inesperado"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _pedidoBusiness.CadastrarPedido(request));
            Assert.Contains("Erro ao cadastrar pedido.", ex.Message);
            _transactionMock.Verify(x => x.RollbackAsync(default), Times.Once);
        }

        [Fact]
        public async Task AtualizarPedido_DeveRetornarSucesso_QuandoAtualizacaoForValida()
        {
            // Arrange
            var request = new PedidoAlteracaoRequestDto
            {
                IdPedido = 1,
                IdUsuario = 1,
                Status = "Atualizado",
                ItensPedido = new List<ItemPedidoAlteracaoRequestDto>
                    {
                        new() { IdItem = 1, IdProduto = 1, Quantidade = 2, Status = "Atualizado" }
                    }
            };
            var pedido = new Pedido { IdPedido = 1, IdUsuario = 1, Status = "Atualizado" };

            _pedidoRepoMock.Setup(x => x.BeginTransactionAsync()).ReturnsAsync(_transactionMock.Object);
            _mapperMock.Setup(x => x.Map<Pedido>(request)).Returns(pedido);
            _pedidoRepoMock.Setup(x => x.AtualizarPedido(It.IsAny<Pedido>())).Returns(Task.CompletedTask);
            _itensPedidoRepoMock.Setup(x => x.AtualizarItensPedido(It.IsAny<List<ItensPedido>>())).Returns(Task.CompletedTask);

            // Act
            await _pedidoBusiness.AtualizarPedido(request);

            // Assert
            _pedidoRepoMock.Verify(x => x.AtualizarPedido(It.IsAny<Pedido>()), Times.Once);
            _itensPedidoRepoMock.Verify(x => x.AtualizarItensPedido(It.IsAny<List<ItensPedido>>()), Times.Once);
            _transactionMock.Verify(x => x.CommitAsync(default), Times.Once);
        }

        [Fact]
        public async Task AtualizarPedido_DeveRetornarFalha_QuandoDbUpdateExceptionForLancada()
        {
            // Arrange
            var request = new PedidoAlteracaoRequestDto
            {
                IdPedido = 1,
                IdUsuario = 1,
                Status = "Atualizado",
                ItensPedido = new List<ItemPedidoAlteracaoRequestDto>
                    {
                        new() { IdItem = 1, IdProduto = 1, Quantidade = 2, Status = "Atualizado" }
                    }
            };
            var pedido = new Pedido { IdPedido = 1, IdUsuario = 1, Status = "Atualizado" };

            _pedidoRepoMock.Setup(x => x.BeginTransactionAsync()).ReturnsAsync(_transactionMock.Object);
            _mapperMock.Setup(x => x.Map<Pedido>(request)).Returns(pedido);
            _pedidoRepoMock.Setup(x => x.AtualizarPedido(It.IsAny<Pedido>())).ThrowsAsync(new DbUpdateException());

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(() => _pedidoBusiness.AtualizarPedido(request));
            _transactionMock.Verify(x => x.RollbackAsync(default), Times.Once);
        }

        [Fact]
        public async Task AtualizarPedido_DeveRetornarFalha_QuandoExceptionForLancada()
        {
            // Arrange
            var request = new PedidoAlteracaoRequestDto
            {
                IdPedido = 1,
                IdUsuario = 1,
                Status = "Atualizado",
                ItensPedido = new List<ItemPedidoAlteracaoRequestDto>
                    {
                        new() { IdItem = 1, IdProduto = 1, Quantidade = 2, Status = "Atualizado" }
                    }
            };
            var pedido = new Pedido { IdPedido = 1, IdUsuario = 1, Status = "Atualizado" };

            _pedidoRepoMock.Setup(x => x.BeginTransactionAsync()).ReturnsAsync(_transactionMock.Object);
            _mapperMock.Setup(x => x.Map<Pedido>(request)).Returns(pedido);
            _pedidoRepoMock.Setup(x => x.AtualizarPedido(It.IsAny<Pedido>())).ThrowsAsync(new Exception("Erro inesperado"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _pedidoBusiness.AtualizarPedido(request));
            Assert.Contains("Erro ao atualizar pedido.", ex.Message);
            _transactionMock.Verify(x => x.RollbackAsync(default), Times.Once);
        }

        [Fact]
        public async Task IncluirItemPedido_DeveRetornarSucesso_QuandoInclusaoForValida()
        {
            // Arrange
            var request = new ItemPedidoRequestDto { IdProduto = 1, Quantidade = 2, IdPedido = 1 };
            var itemPedido = new ItensPedido { IdItem = 1, IdPedido = 1, IdProduto = 1, Quantidade = 2 };
            var response = new ItemPedidoCadastroResponseDto
            {
                IdItem = 1,
                IdProduto = 1,
                Quantidade = 2,
                Status = "Novo"
            };

            _pedidoRepoMock.Setup(x => x.BeginTransactionAsync()).ReturnsAsync(_transactionMock.Object);
            _mapperMock.Setup(x => x.Map<ItensPedido>(request)).Returns(itemPedido);
            _itensPedidoRepoMock.Setup(x => x.IncluirItemPedido(It.IsAny<ItensPedido>())).ReturnsAsync(itemPedido);
            _mapperMock.Setup(x => x.Map<ItemPedidoCadastroResponseDto>(itemPedido)).Returns(response);

            // Act
            var result = await _pedidoBusiness.IncluirItemPedido(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.IdItem);
            Assert.Equal(1, result.IdProduto);
            Assert.Equal(2, result.Quantidade);
            Assert.Equal("Novo", result.Status);
            _itensPedidoRepoMock.Verify(x => x.IncluirItemPedido(It.IsAny<ItensPedido>()), Times.Once);
            _transactionMock.Verify(x => x.CommitAsync(default), Times.Once);
        }

        [Fact]
        public async Task IncluirItemPedido_DeveRetornarFalha_QuandoDbUpdateExceptionForLancada()
        {
            // Arrange
            var request = new ItemPedidoRequestDto { IdProduto = 1, Quantidade = 2, IdPedido = 1 };
            var itemPedido = new ItensPedido { IdItem = 1, IdPedido = 1, IdProduto = 1, Quantidade = 2 };

            _pedidoRepoMock.Setup(x => x.BeginTransactionAsync()).ReturnsAsync(_transactionMock.Object);
            _mapperMock.Setup(x => x.Map<ItensPedido>(request)).Returns(itemPedido);
            _itensPedidoRepoMock.Setup(x => x.IncluirItemPedido(It.IsAny<ItensPedido>())).ThrowsAsync(new DbUpdateException());

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(() => _pedidoBusiness.IncluirItemPedido(request));
            _transactionMock.Verify(x => x.RollbackAsync(default), Times.Once);
        }

        [Fact]
        public async Task IncluirItemPedido_DeveRetornarFalha_QuandoExceptionForLancada()
        {
            // Arrange
            var request = new ItemPedidoRequestDto { IdProduto = 1, Quantidade = 2, IdPedido = 1 };
            var itemPedido = new ItensPedido { IdItem = 1, IdPedido = 1, IdProduto = 1, Quantidade = 2 };

            _pedidoRepoMock.Setup(x => x.BeginTransactionAsync()).ReturnsAsync(_transactionMock.Object);
            _mapperMock.Setup(x => x.Map<ItensPedido>(request)).Returns(itemPedido);
            _itensPedidoRepoMock.Setup(x => x.IncluirItemPedido(It.IsAny<ItensPedido>())).ThrowsAsync(new Exception("Erro inesperado"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _pedidoBusiness.IncluirItemPedido(request));
            Assert.Contains($"Erro ao incluir o produto {request.IdProduto} no pedido {request.IdPedido}.", ex.Message);
            _transactionMock.Verify(x => x.RollbackAsync(default), Times.Once);
        }

        [Fact]
        public async Task ExcluirItemPedido_DeveRetornarSucesso_QuandoExclusaoForValida()
        {
            // Arrange
            int idItem = 1, idPedido = 2;
            _pedidoRepoMock.Setup(x => x.BeginTransactionAsync()).ReturnsAsync(_transactionMock.Object);
            _itensPedidoRepoMock.Setup(x => x.ExcluirItemPedido(idItem, idPedido)).Returns(Task.CompletedTask);

            // Act
            await _pedidoBusiness.ExcluirItemPedido(idItem, idPedido);

            // Assert
            _itensPedidoRepoMock.Verify(x => x.ExcluirItemPedido(idItem, idPedido), Times.Once);
            _transactionMock.Verify(x => x.CommitAsync(default), Times.Once);
        }

        [Fact]
        public async Task ExcluirItemPedido_DeveRetornarFalha_QuandoExceptionForLancada()
        {
            // Arrange
            int idItem = 1, idPedido = 2;
            _pedidoRepoMock.Setup(x => x.BeginTransactionAsync()).ReturnsAsync(_transactionMock.Object);
            _itensPedidoRepoMock.Setup(x => x.ExcluirItemPedido(idItem, idPedido)).ThrowsAsync(new Exception("Erro inesperado"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _pedidoBusiness.ExcluirItemPedido(idItem, idPedido));
            Assert.Contains($"Erro ao excluir item {idItem} do pedido {idPedido}.", ex.Message);
            _transactionMock.Verify(x => x.RollbackAsync(default), Times.Once);
        }

        [Fact]
        public async Task ExcluirPedido_DeveRetornarSucesso_QuandoExclusaoForValida()
        {
            // Arrange
            int idPedido = 1;
            _pedidoRepoMock.Setup(x => x.BeginTransactionAsync()).ReturnsAsync(_transactionMock.Object);
            _itensPedidoRepoMock.Setup(x => x.ExcluirItensPedido(idPedido)).Returns(Task.CompletedTask);
            _pedidoRepoMock.Setup(x => x.ExcluirPedido(idPedido)).Returns(Task.CompletedTask);

            // Act
            await _pedidoBusiness.ExcluirPedido(idPedido);

            // Assert
            _itensPedidoRepoMock.Verify(x => x.ExcluirItensPedido(idPedido), Times.Once);
            _pedidoRepoMock.Verify(x => x.ExcluirPedido(idPedido), Times.Once);
            _transactionMock.Verify(x => x.CommitAsync(default), Times.Once);
        }

        [Fact]
        public async Task ExcluirPedido_DeveRetornarFalha_QuandoExceptionForLancada()
        {
            // Arrange
            int idPedido = 1;
            _pedidoRepoMock.Setup(x => x.BeginTransactionAsync()).ReturnsAsync(_transactionMock.Object);
            _itensPedidoRepoMock.Setup(x => x.ExcluirItensPedido(idPedido)).ThrowsAsync(new Exception("Erro inesperado"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _pedidoBusiness.ExcluirPedido(idPedido));
            Assert.Contains("Erro ao excluir pedido.", ex.Message);
            _transactionMock.Verify(x => x.RollbackAsync(default), Times.Once);
        }

        [Fact]
        public async Task ListarPedidos_DeveRetornarSucesso_QuandoFiltroForValido()
        {
            // Arrange
            var filtro = new PedidoFiltroDto();
            var pedidos = new List<Pedido>
                {
                    new() { IdPedido = 1, IdUsuario = 1, Status = "Novo" }
                };
            var pedidosDto = new List<PedidoResponseDto>
                {
                    new() { IdPedido = 1, IdUsuario = 1, NomeUsuario = "Teste", Status = "Novo", DataCadastro = DateTime.Now, ItensPedido = new List<ItemPedidoResponseDto>() }
                };

            _pedidoRepoMock.Setup(x => x.ListarPedidos(filtro)).ReturnsAsync(pedidos);
            _mapperMock.Setup(x => x.Map<IEnumerable<PedidoResponseDto>>(pedidos)).Returns(pedidosDto);

            // Act
            var result = await _pedidoBusiness.ListarPedidos(filtro);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            _pedidoRepoMock.Verify(x => x.ListarPedidos(filtro), Times.Once);
            _mapperMock.Verify(x => x.Map<IEnumerable<PedidoResponseDto>>(pedidos), Times.Once);
        }

        [Fact]
        public async Task ObterPedidoPorId_DeveRetornarSucesso_QuandoIdForValido()
        {
            // Arrange
            int idPedido = 1;
            var pedido = new Pedido { IdPedido = 1, IdUsuario = 1, Status = "Novo" };
            var pedidoDto = new PedidoResponseDto { IdPedido = 1, IdUsuario = 1, NomeUsuario = "Teste", Status = "Novo", DataCadastro = DateTime.Now, ItensPedido = new List<ItemPedidoResponseDto>() };

            _pedidoRepoMock.Setup(x => x.ObterPedidoPorId(idPedido)).ReturnsAsync(pedido);
            _mapperMock.Setup(x => x.Map<PedidoResponseDto>(pedido)).Returns(pedidoDto);

            // Act
            var result = await _pedidoBusiness.ObterPedidoPorId(idPedido);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.IdPedido);
            _pedidoRepoMock.Verify(x => x.ObterPedidoPorId(idPedido), Times.Once);
            _mapperMock.Verify(x => x.Map<PedidoResponseDto>(pedido), Times.Once);
        }

        [Fact]
        public async Task ObterPedidoPorId_DeveRetornarFalha_QuandoIdForInvalido()
        {
            // Arrange
            int idPedido = 0;

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _pedidoBusiness.ObterPedidoPorId(idPedido));
            Assert.Contains("O ID do usuário deve ser informado para obter.", ex.Message);
        }
    }
}
