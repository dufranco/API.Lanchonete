using API.Lanchonete.Business.Business;
using API.Lanchonete.Domain.DTO;
using API.Lanchonete.Domain.Entities;
using API.Lanchonete.Domain.Interfaces.Repositories;
using AutoMapper;
using Moq;

namespace API.Lanchonete.Tests.Business
{
    public class PerfilBusinessTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IPerfilEFRepository> _perfilRepoMock;
        private readonly PerfilBusiness _perfilBusiness;

        public PerfilBusinessTest()
        {
            _mapperMock = new Mock<IMapper>();
            _perfilRepoMock = new Mock<IPerfilEFRepository>();
            _perfilBusiness = new PerfilBusiness(_mapperMock.Object, _perfilRepoMock.Object);
        }

        [Fact]
        public async Task CadastrarPerfil_DeveRetornarPerfilDto_QuandoSucesso()
        {
            var perfilDto = new PerfilDto { IdPerfil = null, Nome = "Admin", Descricao = "Administrador" };
            var perfilEntity = new Perfil { IdPerfil = 1, Nome = "Admin", Descricao = "Administrador" };
            var perfilDtoResult = new PerfilDto { IdPerfil = 1, Nome = "Admin", Descricao = "Administrador" };

            _perfilRepoMock.Setup(r => r.CadastrarPerfil(perfilDto)).ReturnsAsync(perfilEntity);
            _mapperMock.Setup(m => m.Map<PerfilDto>(perfilEntity)).Returns(perfilDtoResult);

            var result = await _perfilBusiness.CadastrarPerfil(perfilDto);

            Assert.NotNull(result);
            Assert.Equal(1, result.IdPerfil);
            Assert.Equal("Admin", result.Nome);
        }

        [Fact]
        public async Task AtualizarPerfil_DeveChamarRepositorio_QuandoSucesso()
        {
            var perfilDto = new PerfilDto { IdPerfil = 2, Nome = "User", Descricao = "Usuário" };

            _perfilRepoMock.Setup(r => r.AtualizarPerfil(perfilDto)).Returns(Task.CompletedTask);

            await _perfilBusiness.AtualizarPerfil(perfilDto);

            _perfilRepoMock.Verify(r => r.AtualizarPerfil(perfilDto), Times.Once);
        }

        [Fact]
        public async Task AtualizarPerfil_DeveLancarArgumentException_QuandoIdNulo()
        {
            var perfilDto = new PerfilDto { IdPerfil = null, Nome = "User", Descricao = "Usuário" };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _perfilBusiness.AtualizarPerfil(perfilDto));
            Assert.Equal("O ID do perfil deve ser fornecido para atualização.", ex.Message);
        }

        [Fact]
        public async Task ExcluirPerfil_DeveChamarRepositorio_QuandoSucesso()
        {
            int idPerfil = 3;
            _perfilRepoMock.Setup(r => r.ExcluirPerfil(idPerfil)).Returns(Task.CompletedTask);

            await _perfilBusiness.ExcluirPerfil(idPerfil);

            _perfilRepoMock.Verify(r => r.ExcluirPerfil(idPerfil), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task ExcluirPerfil_DeveLancarArgumentException_QuandoIdInvalido(int idPerfil)
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _perfilBusiness.ExcluirPerfil(idPerfil));
            Assert.Equal("O ID do perfil deve ser informado para exclusão.", ex.Message);
        }

        [Fact]
        public async Task ObterPerfilPorId_DeveRetornarPerfilDto_QuandoSucesso()
        {
            int idPerfil = 5;
            var perfilEntity = new Perfil { IdPerfil = idPerfil, Nome = "Gestor", Descricao = "Gestor de loja" };
            var perfilDtoResult = new PerfilDto { IdPerfil = idPerfil, Nome = "Gestor", Descricao = "Gestor de loja" };

            _perfilRepoMock.Setup(r => r.ObterPerfilPorId(idPerfil)).ReturnsAsync(perfilEntity);
            _mapperMock.Setup(m => m.Map<PerfilDto>(perfilEntity)).Returns(perfilDtoResult);

            var result = await _perfilBusiness.ObterPerfilPorId(idPerfil);

            Assert.NotNull(result);
            Assert.Equal(idPerfil, result.IdPerfil);
            Assert.Equal("Gestor", result.Nome);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task ObterPerfilPorId_DeveLancarArgumentException_QuandoIdInvalido(int idPerfil)
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _perfilBusiness.ObterPerfilPorId(idPerfil));
            Assert.Equal("O ID do perfil deve ser informado para obter.", ex.Message);
        }

        [Fact]
        public async Task ListarPerfis_DeveRetornarListaDePerfilDto_QuandoSucesso()
        {
            var perfilFiltro = new PerfilFiltroDto { FiltroNome = null, FiltroDescricao = null, OrdemDesc = false, OrdenarPor = 0, Pagina = 1, TamanhoPagina = 10 };
            var perfisEntity = new List<Perfil>
            {
                new() { IdPerfil = 1, Nome = "Admin", Descricao = "Administrador" },
                new() { IdPerfil = 2, Nome = "User", Descricao = "Usuário" }
            };
            var perfisDto = new List<PerfilDto>
            {
                new() { IdPerfil = 1, Nome = "Admin", Descricao = "Administrador" },
                new() { IdPerfil = 2, Nome = "User", Descricao = "Usuário" }
            };

            _perfilRepoMock.Setup(r => r.ListarPerfis(perfilFiltro)).ReturnsAsync(perfisEntity);
            _mapperMock.Setup(m => m.Map<IEnumerable<PerfilDto>>(perfisEntity)).Returns(perfisDto);

            var result = await _perfilBusiness.ListarPerfis(perfilFiltro);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, p => p.Nome == "Admin");
            Assert.Contains(result, p => p.Nome == "User");
        }
    }
}
