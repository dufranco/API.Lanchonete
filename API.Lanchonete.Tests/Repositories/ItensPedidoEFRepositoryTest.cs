using API.Lanchonete.Data.Context;
using API.Lanchonete.Data.Repositories;
using API.Lanchonete.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Lanchonete.Tests.Repositories
{
    public class ItensPedidoEFRepositoryTest
    {
        private readonly AppDbContext _context;
        private readonly ItensPedidoEFRepository _repository;

        public ItensPedidoEFRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new AppDbContext(options);
            _repository = new ItensPedidoEFRepository(_context);
        }

        private Produto CriarProduto()
        {
            var produto = new Produto
            {
                Nome = "Produto Teste",
                Descricao = "Descricao",
                Preco = 10,
                Tipo = "Comida",
                Ativo = true
            };
            _context.Produtos.Add(produto);
            _context.SaveChanges();
            return produto;
        }

        private Pedido CriarPedido()
        {
            var usuario = new Usuario
            {
                Nome = "Usuario Teste",
                Email = "teste@teste.com",
                SenhaHash = "hash",
                SenhaSalt = "salt"
            };
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            var pedido = new Pedido
            {
                IdUsuario = usuario.IdUsuario,
                Status = "Aberto"
            };
            _context.Pedidos.Add(pedido);
            _context.SaveChanges();
            return pedido;
        }

        [Fact]
        public async Task CadastrarItensPedido_DeveRetornarSucesso_QuandoItensValidos()
        {
            var produto = CriarProduto();
            var pedido = CriarPedido();
            var itens = new List<ItensPedido>
                {
                    new ItensPedido
                    {
                        IdPedido = pedido.IdPedido,
                        IdProduto = produto.IdProduto,
                        Quantidade = 2,
                        Status = "Novo"
                    }
                };

            var result = await _repository.CadastrarItensPedido(itens);

            Assert.Single(result);
            Assert.Equal(pedido.IdPedido, result.First().IdPedido);
        }

        [Fact]
        public async Task AtualizarItensPedido_DeveRetornarSucesso_QuandoItensExistem()
        {
            var produto = CriarProduto();
            var pedido = CriarPedido();
            var item = new ItensPedido
            {
                IdPedido = pedido.IdPedido,
                IdProduto = produto.IdProduto,
                Quantidade = 1,
                Status = "Novo"
            };
            _context.ItensPedido.Add(item);
            _context.SaveChanges();

            item.Quantidade = 5;
            item.Status = "Atualizado";

            await _repository.AtualizarItensPedido(new List<ItensPedido> { item });

            var atualizado = await _context.ItensPedido.FindAsync(item.IdItem);
            Assert.Equal(5, atualizado.Quantidade);
            Assert.Equal("Atualizado", atualizado.Status);
        }

        [Fact]
        public async Task AtualizarItensPedido_DeveRetornarFalha_QuandoItemNaoExiste()
        {
            var item = new ItensPedido
            {
                IdItem = 9999,
                IdPedido = 1,
                IdProduto = 1,
                Quantidade = 1,
                Status = "Novo"
            };

            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _repository.AtualizarItensPedido(new List<ItensPedido> { item }));
            Assert.Contains("não encontrado", ex.Message);
        }

        [Fact]
        public async Task IncluirItemPedido_DeveRetornarSucesso_QuandoItemValido()
        {
            var produto = CriarProduto();
            var pedido = CriarPedido();
            var item = new ItensPedido
            {
                IdPedido = pedido.IdPedido,
                IdProduto = produto.IdProduto,
                Quantidade = 1,
                Status = "Novo"
            };

            await _repository.IncluirItemPedido(item);

            var existe = await _context.ItensPedido.AnyAsync(i => i.IdPedido == pedido.IdPedido && i.IdProduto == produto.IdProduto);
            Assert.True(existe);
        }

        [Fact]
        public async Task IncluirItemPedido_DeveRetornarFalha_QuandoPedidoNaoExiste()
        {
            var produto = CriarProduto();
            var item = new ItensPedido
            {
                IdPedido = 9999,
                IdProduto = produto.IdProduto,
                Quantidade = 1,
                Status = "Novo"
            };

            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _repository.IncluirItemPedido(item));
            Assert.Contains("não foi encontrado", ex.Message);
        }

        [Fact]
        public async Task IncluirItemPedido_DeveRetornarFalha_QuandoProdutoJaIncluido()
        {
            var produto = CriarProduto();
            var pedido = CriarPedido();
            var item = new ItensPedido
            {
                IdPedido = pedido.IdPedido,
                IdProduto = produto.IdProduto,
                Quantidade = 1,
                Status = "Novo"
            };
            _context.ItensPedido.Add(item);
            _context.SaveChanges();

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _repository.IncluirItemPedido(item));
            Assert.Contains("já está incluído", ex.Message);
        }

        [Fact]
        public async Task ExcluirItemPedido_DeveRetornarSucesso_QuandoItemExiste()
        {
            var produto = CriarProduto();
            var pedido = CriarPedido();
            var item = new ItensPedido
            {
                IdPedido = pedido.IdPedido,
                IdProduto = produto.IdProduto,
                Quantidade = 1,
                Status = "Novo"
            };
            _context.ItensPedido.Add(item);
            _context.SaveChanges();

            await _repository.ExcluirItemPedido(item.IdItem, item.IdPedido);

            var existe = await _context.ItensPedido.AnyAsync(i => i.IdItem == item.IdItem);
            Assert.False(existe);
        }

        [Fact]
        public async Task ExcluirItemPedido_DeveRetornarFalha_QuandoItemNaoExiste()
        {
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _repository.ExcluirItemPedido(9999, 1));
            Assert.Contains("não encontrado", ex.Message);
        }

        [Fact]
        public async Task ExcluirItensPedido_DeveRetornarSucesso_QuandoItensExistem()
        {
            var produto = CriarProduto();
            var pedido = CriarPedido();
            var item1 = new ItensPedido
            {
                IdPedido = pedido.IdPedido,
                IdProduto = produto.IdProduto,
                Quantidade = 1,
                Status = "Novo"
            };
            var item2 = new ItensPedido
            {
                IdPedido = pedido.IdPedido,
                IdProduto = produto.IdProduto,
                Quantidade = 2,
                Status = "Novo"
            };
            _context.ItensPedido.AddRange(item1, item2);
            _context.SaveChanges();

            await _repository.ExcluirItensPedido(pedido.IdPedido);

            var existe = await _context.ItensPedido.AnyAsync(i => i.IdPedido == pedido.IdPedido);
            Assert.False(existe);
        }

        [Fact]
        public async Task ExcluirItensPedidoComIdItem_DeveRetornarSucesso_QuandoItemExiste()
        {
            var produto = CriarProduto();
            var pedido = CriarPedido();
            var item = new ItensPedido
            {
                IdPedido = pedido.IdPedido,
                IdProduto = produto.IdProduto,
                Quantidade = 1,
                Status = "Novo"
            };
            _context.ItensPedido.Add(item);
            _context.SaveChanges();

            await _repository.ExcluirItensPedido(pedido.IdPedido, item.IdItem);

            var existe = await _context.ItensPedido.AnyAsync(i => i.IdItem == item.IdItem);
            Assert.False(existe);
        }

        [Fact]
        public async Task ExcluirItensPedidoComIdItem_DeveRetornarFalha_QuandoItemNaoExiste()
        {
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _repository.ExcluirItensPedido(9999, 8888));
            Assert.Contains("Nenhum item encontrado", ex.Message);
        }
    }
}
