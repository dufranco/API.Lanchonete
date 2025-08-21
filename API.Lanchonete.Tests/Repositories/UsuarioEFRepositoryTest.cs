using API.Lanchonete.Data.Context;
using API.Lanchonete.Data.Repositories;
using API.Lanchonete.Domain.DTO;
using API.Lanchonete.Domain.DTO.Request.Filtro;
using API.Lanchonete.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace API.Lanchonete.Tests.Repositories
{
    public class UsuarioEFRepositoryTest
    {
        private readonly AppDbContext _context;
        private readonly UsuarioEFRepository _repository;

        public UsuarioEFRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new AppDbContext(options);

            // Seed Perfil
            var perfil = new Perfil { IdPerfil = 1, Nome = "Admin", Descricao = "Administrador" };
            _context.Perfis.Add(perfil);
            _context.SaveChanges();

            _repository = new UsuarioEFRepository(_context);
        }

        [Fact]
        public async Task CadastrarUsuario_DeveRetornarSucesso_QuandoUsuarioValido()
        {
            var usuario = new Usuario
            {
                Nome = "Teste",
                Email = "teste@email.com",
                IdPerfil = 1,
                SenhaHash = "hash",
                SenhaSalt = "salt"
            };

            var result = await _repository.CadastrarUsuario(usuario);

            Assert.NotNull(result);
            Assert.Equal("Teste", result.Nome);
            Assert.Equal("teste@email.com", result.Email);
        }

        [Fact]
        public async Task AtualizarUsuario_DeveRetornarSucesso_QuandoUsuarioValido()
        {
            var usuario = new Usuario
            {
                Nome = "Original",
                Email = "original@email.com",
                IdPerfil = 1,
                SenhaHash = "hash",
                SenhaSalt = "salt"
            };
            await _repository.CadastrarUsuario(usuario);

            usuario.Nome = "Atualizado";
            usuario.Email = "atualizado@email.com";
            usuario.SenhaHash = "newhash";
            usuario.SenhaSalt = "newsalt";

            await _repository.AtualizarUsuario(usuario);

            var atualizado = await _context.Usuarios.FindAsync(usuario.IdUsuario);
            Assert.NotNull(atualizado);
            Assert.Equal("Atualizado", atualizado.Nome);
            Assert.Equal("atualizado@email.com", atualizado.Email);
            Assert.Equal("newhash", atualizado.SenhaHash);
            Assert.Equal("newsalt", atualizado.SenhaSalt);
        }

        [Fact]
        public async Task AtualizarUsuario_DeveRetornarFalha_QuandoUsuarioNaoExiste()
        {
            var usuario = new Usuario
            {
                IdUsuario = 999,
                Nome = "Inexistente",
                Email = "inexistente@email.com",
                IdPerfil = 1,
                SenhaHash = "hash",
                SenhaSalt = "salt"
            };

            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _repository.AtualizarUsuario(usuario)
            );
        }

        [Fact]
        public async Task ExcluirUsuario_DeveRetornarSucesso_QuandoUsuarioSemPedidos()
        {
            var usuario = new Usuario
            {
                Nome = "Excluir",
                Email = "excluir@email.com",
                IdPerfil = 1,
                SenhaHash = "hash",
                SenhaSalt = "salt"
            };

            await _repository.CadastrarUsuario(usuario);
            await _repository.ExcluirUsuario(usuario.IdUsuario);

            var excluido = await _context.Usuarios.FindAsync(usuario.IdUsuario);
            Assert.Null(excluido);
        }

        [Fact]
        public async Task ExcluirUsuario_DeveRetornarFalha_QuandoUsuarioNaoExiste()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _repository.ExcluirUsuario(999)
            );
        }

        [Fact]
        public async Task ExcluirUsuario_DeveRetornarFalha_QuandoUsuarioComPedidos()
        {
            var usuario = new Usuario
            {
                Nome = "ComPedido",
                Email = "compedido@email.com",
                IdPerfil = 1,
                SenhaHash = "hash",
                SenhaSalt = "salt"
            };

            await _repository.CadastrarUsuario(usuario);

            var pedido = new Pedido
            {
                IdUsuario = usuario.IdUsuario,
                Status = "Novo"
            };
            _context.Pedidos.Add(pedido);

            await _context.SaveChangesAsync();
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _repository.ExcluirUsuario(usuario.IdUsuario)
            );
        }

        [Fact]
        public async Task ObterUsuarioPorId_DeveRetornarSucesso_QuandoUsuarioExiste()
        {
            var usuario = new Usuario
            {
                Nome = "Buscar",
                Email = "buscar@email.com",
                IdPerfil = 1,
                SenhaHash = "hash",
                SenhaSalt = "salt"
            };

            await _repository.CadastrarUsuario(usuario);

            var result = await _repository.ObterUsuarioPorId(usuario.IdUsuario);

            Assert.NotNull(result);
            Assert.Equal("Buscar", result.Nome);
        }

        [Fact]
        public async Task ObterUsuarioPorId_DeveRetornarFalha_QuandoUsuarioNaoExiste()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _repository.ObterUsuarioPorId(999)
            );
        }

        [Fact]
        public async Task ListarUsuarios_DeveRetornarSucesso_QuandoUsuariosExistem()
        {
            var usuario1 = new Usuario
            {
                Nome = "Listar1",
                Email = "listar1@email.com",
                IdPerfil = 1,
                SenhaHash = "hash1",
                SenhaSalt = "salt1"
            };
            var usuario2 = new Usuario
            {
                Nome = "Listar2",
                Email = "listar2@email.com",
                IdPerfil = 1,
                SenhaHash = "hash2",
                SenhaSalt = "salt2"
            };
            await _repository.CadastrarUsuario(usuario1);
            await _repository.CadastrarUsuario(usuario2);

            var filtro = new UsuarioFiltroDto
            {
                Nome = "Listar",
                Pagina = 1,
                TamanhoPagina = 10,
                OrdemDesc = false
            };

            var result = await _repository.ListarUsuarios(filtro);

            Assert.NotEmpty(result);
            Assert.Contains(result, u => u.Nome.StartsWith("Listar"));
        }

        [Fact]
        public async Task ListarUsuarios_DeveRetornarFalha_QuandoNenhumUsuarioEncontrado()
        {
            var filtro = new UsuarioFiltroDto
            {
                Nome = "Inexistente",
                Pagina = 1,
                TamanhoPagina = 10,
                OrdemDesc = false
            };

            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _repository.ListarUsuarios(filtro));
        }

        [Fact]
        public async Task CadastrarUsuario_DeveRetornarFalha_QuandoSaveChangesAsyncLancaDbUpdateException()
        {
            var usuario = new Usuario
            {
                Nome = "MockException",
                Email = "mock@email.com",
                IdPerfil = 1,
                SenhaHash = "hash",
                SenhaSalt = "salt"
            };

            var mockContext = new Mock<AppDbContext>(new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            mockContext.Setup(c => c.Usuarios.AddAsync(It.IsAny<Usuario>(), default))
                .ReturnsAsync((Usuario u, CancellationToken ct) => { return null; });

            mockContext.Setup(c => c.SaveChangesAsync(default)).ThrowsAsync(
                new DbUpdateException("Erro mock", new Exception("usuarios_email_key"))
            );

            var repository = new UsuarioEFRepository(mockContext.Object);

            var ex = await Assert.ThrowsAsync<DbUpdateException>(async () =>
                await repository.CadastrarUsuario(usuario)
            );

            Assert.Contains("Já existe um usuário com o e-mail", ex.Message);
        }

        [Fact]
        public async Task CadastrarUsuario_DeveRetornarFalha_QuandoSaveChangesAsyncLancaDbUpdateException_PerfilFK()
        {
            var usuario = new Usuario
            {
                Nome = "MockPerfilFK",
                Email = "mockperfil@email.com",
                IdPerfil = 999, // perfil inexistente
                SenhaHash = "hash",
                SenhaSalt = "salt"
            };

            var mockContext = new Mock<AppDbContext>(new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

            mockContext.Setup(c => c.Usuarios.AddAsync(It.IsAny<Usuario>(), default))
                .ReturnsAsync((Usuario u, CancellationToken ct) => { return null; });

            mockContext.Setup(c => c.SaveChangesAsync(default)).ThrowsAsync(
                new DbUpdateException("Erro mock", new Exception("usuarios_id_perfil_fkey"))
            );

            var repository = new UsuarioEFRepository(mockContext.Object);

            var ex = await Assert.ThrowsAsync<DbUpdateException>(async () =>
                await repository.CadastrarUsuario(usuario)
            );

            Assert.Contains($"Perfil com código {usuario.IdPerfil} não encontrado.", ex.Message);
        }
    }
}
