using API.Lanchonete.Data.Context;
using API.Lanchonete.Data.Repositories;
using API.Lanchonete.Domain.DTO.Request.Filtro;
using API.Lanchonete.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Lanchonete.Tests.Repositories
{
    public class ControleAcessoEFRepositoryTest
    {
        private readonly AppDbContext _context;
        private readonly ControleAcessoEFRepository _repository;

        public ControleAcessoEFRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new AppDbContext(options);
            _repository = new ControleAcessoEFRepository(_context);
        }

        private Perfil CriarPerfil(string nome = "Perfil Teste")
        {
            var perfil = new Perfil
            {
                Nome = nome,
                Descricao = "Descrição",
                DataCadastro = DateTime.UtcNow
            };
            _context.Perfis.Add(perfil);
            _context.SaveChanges();
            return perfil;
        }

        [Fact]
        public async Task CadastrarControleAcesso_DeveRetornarSucesso_QuandoCadastroValido()
        {
            var perfil = CriarPerfil();
            var controleAcesso = new ControleAcesso
            {
                IdPerfil = perfil.IdPerfil,
                NomeTela = "TelaTeste",
                Permitido = true,
                DataCadastro = DateTime.UtcNow
            };

            var result = await _repository.CadastrarControleAcesso(controleAcesso);

            Assert.NotNull(result);
            Assert.True(result.IdControle > 0);
            Assert.Equal("TelaTeste", result.NomeTela);
        }

        [Fact]
        public async Task AtualizarControleAcesso_DeveRetornarSucesso_QuandoAtualizacaoValida()
        {
            var perfil = CriarPerfil();
            var controleAcesso = new ControleAcesso
            {
                IdPerfil = perfil.IdPerfil,
                NomeTela = "TelaOriginal",
                Permitido = true,
                DataCadastro = DateTime.UtcNow
            };
            _context.ControleAcesso.Add(controleAcesso);
            _context.SaveChanges();

            controleAcesso.NomeTela = "TelaAtualizada";
            controleAcesso.Permitido = false;

            await _repository.AtualizarControleAcesso(controleAcesso);

            var atualizado = await _context.ControleAcesso.FindAsync(controleAcesso.IdControle);
            Assert.Equal("TelaAtualizada", atualizado.NomeTela);
            Assert.False(atualizado.Permitido);
        }

        [Fact]
        public async Task AtualizarControleAcesso_DeveRetornarFalha_QuandoControleAcessoNaoExiste()
        {
            var perfil = CriarPerfil();
            var controleAcesso = new ControleAcesso
            {
                IdControle = 9999,
                IdPerfil = perfil.IdPerfil,
                NomeTela = "Inexistente",
                Permitido = false,
                DataCadastro = DateTime.UtcNow
            };

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _repository.AtualizarControleAcesso(controleAcesso));
        }

        [Fact]
        public async Task ExcluirControleAcesso_DeveRetornarSucesso_QuandoControleAcessoExiste()
        {
            var perfil = CriarPerfil();
            var controleAcesso = new ControleAcesso
            {
                IdPerfil = perfil.IdPerfil,
                NomeTela = "TelaExcluir",
                Permitido = true,
                DataCadastro = DateTime.UtcNow
            };
            _context.ControleAcesso.Add(controleAcesso);
            _context.SaveChanges();

            await _repository.ExcluirControleAcesso(controleAcesso.IdControle);

            var excluido = await _context.ControleAcesso.FindAsync(controleAcesso.IdControle);
            Assert.Null(excluido);
        }

        [Fact]
        public async Task ExcluirControleAcesso_DeveRetornarFalha_QuandoControleAcessoNaoExiste()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _repository.ExcluirControleAcesso(9999));
        }

        [Fact]
        public async Task ListarControleAcessos_DeveRetornarSucesso_QuandoFiltroValido()
        {
            var perfil = CriarPerfil("PerfilFiltro");
            _context.ControleAcesso.Add(new ControleAcesso
            {
                IdPerfil = perfil.IdPerfil,
                NomeTela = "TelaFiltro",
                Permitido = true,
                DataCadastro = DateTime.UtcNow
            });
            _context.SaveChanges();

            var filtro = new ControleAcessoFiltroDto
            {
                IdPerfil = perfil.IdPerfil,
                Pagina = 1,
                TamanhoPagina = 10,
                OrdenarPor = 1,
                OrdemDesc = false
            };

            var result = await _repository.ListarControleAcessos(filtro);

            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.Contains(result, x => x.NomeTela == "TelaFiltro");
        }

        [Fact]
        public async Task ListarControleAcessos_DeveRetornarSucesso_QuandoNenhumRegistroEncontrado()
        {
            var filtro = new ControleAcessoFiltroDto
            {
                IdPerfil = 9999,
                Pagina = 1,
                TamanhoPagina = 10,
                OrdenarPor = 1,
                OrdemDesc = false
            };

            var result = await _repository.ListarControleAcessos(filtro);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task ObterControleAcessoPorId_DeveRetornarSucesso_QuandoControleAcessoExiste()
        {
            var perfil = CriarPerfil();
            var controleAcesso = new ControleAcesso
            {
                IdPerfil = perfil.IdPerfil,
                NomeTela = "TelaObter",
                Permitido = true,
                DataCadastro = DateTime.UtcNow
            };
            _context.ControleAcesso.Add(controleAcesso);
            _context.SaveChanges();

            var result = await _repository.ObterControleAcessoPorId(controleAcesso.IdControle);

            Assert.NotNull(result);
            Assert.Equal("TelaObter", result.NomeTela);
        }

        [Fact]
        public async Task ObterControleAcessoPorId_DeveRetornarFalha_QuandoControleAcessoNaoExiste()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _repository.ObterControleAcessoPorId(9999));
        }
    }
}
