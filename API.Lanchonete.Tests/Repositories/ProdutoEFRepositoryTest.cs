using API.Lanchonete.Data.Context;
using API.Lanchonete.Data.Repositories;
using API.Lanchonete.Domain.DTO.Request.Filtro;
using API.Lanchonete.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace API.Lanchonete.Tests.Repositories
{
    public class ProdutoEFRepositoryTest
    {
        private readonly AppDbContext _context;
        private readonly ProdutoEFRepository _repository;

        public ProdutoEFRepositoryTest()
        {
            _context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            _repository = new ProdutoEFRepository(_context);
        }

        [Fact]
        public async Task CadastrarProduto_DeveRetornarSucesso_QuandoProdutoValido()
        {
            var produto = new Produto
            {
                Nome = "Coca-Cola",
                Descricao = "Refrigerante",
                Preco = 5.0m,
                Tipo = "bebida",
                Ativo = true
            };

            var result = await _repository.CadastrarProduto(produto);

            Assert.NotNull(result);
            Assert.Equal("Coca-Cola", result.Nome);
            Assert.True(result.IdProduto > 0);
        }

        [Fact]
        public async Task AtualizarProduto_DeveRetornarSucesso_QuandoProdutoExiste()
        {
            var produto = new Produto
            {
                Nome = "Prato",
                Descricao = "Comida",
                Preco = 20.0m,
                Tipo = "prato",
                Ativo = true
            };
            await _repository.CadastrarProduto(produto);

            produto.Nome = "Prato Atualizado";
            await _repository.AtualizarProduto(produto);

            var atualizado = await _repository.ObterProdutoPorId(produto.IdProduto);
            Assert.Equal("Prato Atualizado", atualizado.Nome);
        }

        [Fact]
        public async Task AtualizarProduto_DeveRetornarFalha_QuandoProdutoNaoExiste()
        {
            var produto = new Produto
            {
                IdProduto = 999,
                Nome = "Inexistente",
                Descricao = "Teste",
                Preco = 1.0m,
                Tipo = "prato",
                Ativo = true
            };

            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await _repository.AtualizarProduto(produto);
            });
        }

        [Fact]
        public async Task ExcluirProduto_DeveRetornarSucesso_QuandoProdutoSemAssociacao()
        {
            var produto = new Produto
            {
                Nome = "Bebida",
                Descricao = "Água",
                Preco = 2.0m,
                Tipo = "bebida",
                Ativo = true
            };
            await _repository.CadastrarProduto(produto);

            await _repository.ExcluirProduto(produto.IdProduto);

            Assert.False(_context.Produtos.Any(p => p.IdProduto == produto.IdProduto));
        }

        [Fact]
        public async Task ExcluirProduto_DeveRetornarFalha_QuandoProdutoComAssociacao()
        {
            var produto = new Produto
            {
                Nome = "Prato",
                Descricao = "Comida",
                Preco = 15.0m,
                Tipo = "prato",
                Ativo = true
            };
            await _repository.CadastrarProduto(produto);

            _context.ItensPedido.Add(new ItensPedido
            {
                IdProduto = produto.IdProduto,
                Quantidade = 1,
                Status = "Novo"
            });
            await _context.SaveChangesAsync();

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _repository.ExcluirProduto(produto.IdProduto);
            });
        }

        [Fact]
        public async Task ExcluirProduto_DeveRetornarFalha_QuandoProdutoNaoExiste()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await _repository.ExcluirProduto(999);
            });
        }

        [Fact]
        public async Task ObterProdutoPorId_DeveRetornarSucesso_QuandoProdutoExiste()
        {
            var produto = new Produto
            {
                Nome = "Suco",
                Descricao = "Natural",
                Preco = 7.0m,
                Tipo = "bebida",
                Ativo = true
            };
            await _repository.CadastrarProduto(produto);

            var result = await _repository.ObterProdutoPorId(produto.IdProduto);

            Assert.NotNull(result);
            Assert.Equal(produto.IdProduto, result.IdProduto);
        }

        [Fact]
        public async Task ObterProdutoPorId_DeveRetornarFalha_QuandoProdutoNaoExiste()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await _repository.ObterProdutoPorId(999);
            });
        }

        [Fact]
        public async Task ListarProdutos_DeveRetornarSucesso_QuandoProdutosExistem()
        {
            await _repository.CadastrarProduto(new Produto
            {
                Nome = "Prato 1",
                Descricao = "Comida 1",
                Preco = 10.0m,
                Tipo = "prato",
                Ativo = true
            });
            await _repository.CadastrarProduto(new Produto
            {
                Nome = "Prato 2",
                Descricao = "Comida 2",
                Preco = 12.0m,
                Tipo = "prato",
                Ativo = true
            });

            var filtro = new ProdutoFiltroDto
            {
                Pagina = 1,
                TamanhoPagina = 10
            };

            var result = await _repository.ListarProdutos(filtro);

            Assert.NotEmpty(result);
            Assert.True(result.Count() >= 2);
        }

        [Fact]
        public async Task ListarProdutos_DeveRetornarFalha_QuandoNenhumProdutoEncontrado()
        {
            var filtro = new ProdutoFiltroDto
            {
                Nome = "Inexistente",
                Pagina = 1,
                TamanhoPagina = 10
            };

            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await _repository.ListarProdutos(filtro);
            });
        }
    }
}
