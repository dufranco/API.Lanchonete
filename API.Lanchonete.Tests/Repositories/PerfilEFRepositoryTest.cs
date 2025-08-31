using API.Lanchonete.Data.Context;
using API.Lanchonete.Data.Repositories;
using API.Lanchonete.Domain.DTO;
using API.Lanchonete.Domain.DTO.Request.Filtro;
using API.Lanchonete.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Lanchonete.Tests.Repositories
{
    public class PerfilEFRepositoryTest
    {
        private readonly AppDbContext _context;
        public PerfilEFRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;
            _context = new AppDbContext(options);
        }

        [Fact]
        public async Task CadastrarPerfil_DeveRetornarSucesso_QuandoPerfilForValido()
        {
            var repo = new PerfilEFRepository(_context);
            var dto = new PerfilDto { Nome = "Admin", Descricao = "Administrador" };

            var perfil = await repo.CadastrarPerfil(dto);

            Assert.NotNull(perfil);
            Assert.Equal("Admin", perfil.Nome);
            Assert.Equal("Administrador", perfil.Descricao);
        }

        [Fact]
        public async Task AtualizarPerfil_Success()
        {
            var repo = new PerfilEFRepository(_context);
            var dto = new PerfilDto { Nome = "User", Descricao = "Usuário" };
            var perfil = await repo.CadastrarPerfil(dto);

            var updateDto = new PerfilDto { IdPerfil = perfil.IdPerfil, Nome = "User2", Descricao = "Usuário 2" };
            await repo.AtualizarPerfil(updateDto);

            var updated = await _context.Perfis.FindAsync(perfil.IdPerfil);
            Assert.NotNull(updated);
            Assert.Equal("User2", updated!.Nome);
            Assert.Equal("Usuário 2", updated!.Descricao);
        }

        [Fact]
        public async Task AtualizarPerfil_Fail_NotFound()
        {
            var repo = new PerfilEFRepository(_context);
            var dto = new PerfilDto { IdPerfil = 999, Nome = "X", Descricao = "Y" };

            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await repo.AtualizarPerfil(dto);
            });
        }

        [Fact]
        public async Task ExcluirPerfil_Success()
        {
            var repo = new PerfilEFRepository(_context);
            var dto = new PerfilDto { Nome = "Excluir", Descricao = "Excluir" };
            var perfil = await repo.CadastrarPerfil(dto);

            await repo.ExcluirPerfil(perfil.IdPerfil);

            var deleted = await _context.Perfis.FindAsync(perfil.IdPerfil);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task ExcluirPerfil_Fail_NotFound()
        {
            var repo = new PerfilEFRepository(_context);

            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await repo.ExcluirPerfil(999);
            });
        }

        [Fact]
        public async Task ExcluirPerfil_Fail_HasAssociations()
        {
            var repo = new PerfilEFRepository(_context);
            var perfil = await repo.CadastrarPerfil(new PerfilDto { Nome = "Associado", Descricao = "Teste" });

            _context.Usuarios.Add(new Usuario { IdPerfil = perfil.IdPerfil, Nome = "U", Email = "u@u.com", SenhaHash = "h", SenhaSalt = "s" });
            await _context.SaveChangesAsync();

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await repo.ExcluirPerfil(perfil.IdPerfil);
            });
        }

        [Fact]
        public async Task ObterPerfilPorId_DeveRetornarSucesso_QuandoPerfilExistir()
        {
            var repo = new PerfilEFRepository(_context);
            var dto = new PerfilDto { Nome = "Buscar", Descricao = "Buscar" };
            var perfil = await repo.CadastrarPerfil(dto);

            var result = await repo.ObterPerfilPorId(perfil.IdPerfil);

            Assert.NotNull(result);
            Assert.Equal(perfil.IdPerfil, result!.IdPerfil);
        }

        [Fact]
        public async Task ObterPerfilPorId_DeveRetornarFalha_QuandoPerfilNaoExistir()
        {
            var repo = new PerfilEFRepository(_context);

            await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await repo.ObterPerfilPorId(999);
            });
        }

        [Fact]
        public async Task ListarPerfis_DeveRetornarSucesso_QuandoNaoHouverPerfis()
        {
            var repo = new PerfilEFRepository(_context);
            var filtro = new PerfilFiltroDto { Nome = "Inexistente" };

            var result = await repo.ListarPerfis(filtro);

            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
