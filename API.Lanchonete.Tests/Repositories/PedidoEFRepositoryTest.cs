using API.Lanchonete.Data.Context;
using API.Lanchonete.Data.Repositories;
using API.Lanchonete.Domain.DTO;
using API.Lanchonete.Domain.DTO.Request.Filtro;
using API.Lanchonete.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Lanchonete.Tests.Repositories
{
    public class PedidoEFRepositoryTest
    {
        private readonly AppDbContext _context;
        private readonly PedidoEFRepository _repository;

        public PedidoEFRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new AppDbContext(options);
            _repository = new PedidoEFRepository(_context);
        }

        #region CadastrarPedido

        [Fact]
        public async Task CadastrarPedido_DeveRetornarSucesso_QuandoPedidoValido()
        {
            var usuario = new Usuario { IdUsuario = 1, Nome = "Teste", Email = "teste@teste.com", SenhaHash = "hash", SenhaSalt = "salt" };
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var pedido = new Pedido
            {
                IdUsuario = usuario.IdUsuario,
                Status = "Novo",
                DataCadastro = DateTime.Now,
                ItensPedidos = new List<ItensPedido>()
            };

            var result = await _repository.CadastrarPedido(pedido);

            Assert.NotNull(result);
            Assert.Equal(usuario.IdUsuario, result.IdUsuario);
            Assert.True(result.IdPedido > 0);
        }

        #endregion

        #region AtualizarPedido

        [Fact]
        public async Task AtualizarPedido_DeveRetornarSucesso_QuandoPedidoExiste()
        {
            var usuario = new Usuario { IdUsuario = 2, Nome = "Teste2", Email = "teste2@teste.com", SenhaHash = "hash", SenhaSalt = "salt" };
            _context.Usuarios.Add(usuario);
            var pedido = new Pedido
            {
                IdUsuario = usuario.IdUsuario,
                Status = "Novo",
                DataCadastro = DateTime.Now,
                ItensPedidos = new List<ItensPedido>()
            };
            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            pedido.Status = "Finalizado";
            await _repository.AtualizarPedido(pedido);

            var atualizado = await _context.Pedidos.FindAsync(pedido.IdPedido);
            Assert.Equal("Finalizado", atualizado.Status);
        }

        [Fact]
        public async Task AtualizarPedido_DeveRetornarFalha_QuandoPedidoNaoExiste()
        {
            var pedido = new Pedido
            {
                IdPedido = 999,
                IdUsuario = 1,
                Status = "Novo"
            };

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _repository.AtualizarPedido(pedido));
        }

        #endregion

        #region ExcluirPedido

        [Fact]
        public async Task ExcluirPedido_DeveRetornarSucesso_QuandoPedidoExiste()
        {
            var usuario = new Usuario { IdUsuario = 3, Nome = "Teste3", Email = "teste3@teste.com", SenhaHash = "hash", SenhaSalt = "salt" };
            _context.Usuarios.Add(usuario);
            var pedido = new Pedido
            {
                IdUsuario = usuario.IdUsuario,
                Status = "Novo",
                DataCadastro = DateTime.Now,
                ItensPedidos = new List<ItensPedido>()
            };
            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            await _repository.ExcluirPedido(pedido.IdPedido);

            var excluido = await _context.Pedidos.FindAsync(pedido.IdPedido);
            Assert.Null(excluido);
        }

        [Fact]
        public async Task ExcluirPedido_DeveRetornarFalha_QuandoPedidoNaoExiste()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _repository.ExcluirPedido(999));
        }

        #endregion

        #region ListarPedidos

        [Fact]
        public async Task ListarPedidos_DeveRetornarSucesso_QuandoFiltroValido()
        {
            var usuario = new Usuario { IdUsuario = 4, Nome = "Teste4", Email = "teste4@teste.com", SenhaHash = "hash", SenhaSalt = "salt" };
            _context.Usuarios.Add(usuario);
            var pedido = new Pedido
            {
                IdUsuario = usuario.IdUsuario,
                Status = "Novo",
                DataCadastro = DateTime.Now,
                ItensPedidos = new List<ItensPedido>()
            };
            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            var filtro = new PedidoFiltroDto
            {
                Pagina = 1,
                TamanhoPagina = 10,
                OrdenarPor = 1,
                OrdemDesc = false
            };

            var result = await _repository.ListarPedidos(filtro);

            Assert.NotNull(result);
            Assert.Contains(result, p => p.IdPedido == pedido.IdPedido);
        }

        [Fact]
        public async Task ListarPedidos_DeveRetornarSucesso_QuandoNenhumPedidoEncontrado()
        {
            var filtro = new PedidoFiltroDto
            {
                Pagina = 1,
                TamanhoPagina = 10,
                OrdenarPor = 1,
                OrdemDesc = false,
                IdPedido = 999
            };

            var result = await _repository.ListarPedidos(filtro);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region ObterPedidoPorId

        [Fact]
        public async Task ObterPedidoPorId_DeveRetornarSucesso_QuandoPedidoExiste()
        {
            var usuario = new Usuario { IdUsuario = 5, Nome = "Teste5", Email = "teste5@teste.com", SenhaHash = "hash", SenhaSalt = "salt" };
            _context.Usuarios.Add(usuario);
            var pedido = new Pedido
            {
                IdUsuario = usuario.IdUsuario,
                Status = "Novo",
                DataCadastro = DateTime.Now,
                ItensPedidos = new List<ItensPedido>()
            };
            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            var result = await _repository.ObterPedidoPorId(pedido.IdPedido);

            Assert.NotNull(result);
            Assert.Equal(pedido.IdPedido, result.IdPedido);
        }

        [Fact]
        public async Task ObterPedidoPorId_DeveRetornarFalha_QuandoPedidoNaoExiste()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _repository.ObterPedidoPorId(999));
        }

        #endregion
    }
}
