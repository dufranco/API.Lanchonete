using API.Lanchonete.Business.Business;
using API.Lanchonete.Domain.DTO;
using API.Lanchonete.Domain.DTO.Request;
using API.Lanchonete.Domain.DTO.Request.Filtro;
using API.Lanchonete.Domain.DTO.Response;
using API.Lanchonete.Domain.Entities;
using API.Lanchonete.Domain.Interfaces.Repositories;
using AutoMapper;
using Moq;

namespace API.Lanchonete.Tests.Business
{
    public class UsuarioBusinessTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUsuarioEFRepository> _usuarioRepoMock;
        private readonly UsuarioBusiness _usuarioBusiness;

        public UsuarioBusinessTest()
        {
            _mapperMock = new Mock<IMapper>();
            _usuarioRepoMock = new Mock<IUsuarioEFRepository>();
            _usuarioBusiness = new UsuarioBusiness(_mapperMock.Object, _usuarioRepoMock.Object);
        }

        [Fact]
        public async Task CadastrarUsuario_DeveRetornarSucesso_QuandoUsuarioValido()
        {
            var request = new UsuarioCadastroRequestDto { Nome = "Teste", Email = "teste@email.com", IdPerfil = 1, Senha = "123" };
            var usuario = new Usuario { IdUsuario = 1, Nome = "Teste", Email = "teste@email.com", IdPerfil = 1 };
            var response = new UsuarioCadastroResponseDto { IdUsuario = 1, Nome = "Teste", Email = "teste@email.com", IdPerfil = 1 };

            _mapperMock.Setup(m => m.Map<Usuario>(request)).Returns(usuario);
            _usuarioRepoMock.Setup(r => r.CadastrarUsuario(usuario)).ReturnsAsync(usuario);
            _mapperMock.Setup(m => m.Map<UsuarioCadastroResponseDto>(usuario)).Returns(response);

            var result = await _usuarioBusiness.CadastrarUsuario(request);

            Assert.NotNull(result);
            Assert.Equal(1, result.IdUsuario);
        }

        [Fact]
        public async Task AtualizarUsuario_DeveRetornarSucesso_QuandoUsuarioValido()
        {
            var request = new UsuarioAlteracaoRequestDto { IdUsuario = 1, Nome = "Teste", Email = "teste@email.com", IdPerfil = 1 };
            var usuario = new Usuario { IdUsuario = 1, Nome = "Teste", Email = "teste@email.com", IdPerfil = 1 };

            _mapperMock.Setup(m => m.Map<Usuario>(request)).Returns(usuario);
            _usuarioRepoMock.Setup(r => r.AtualizarUsuario(usuario)).Returns(Task.CompletedTask);

            await _usuarioBusiness.AtualizarUsuario(request);

            _usuarioRepoMock.Verify(r => r.AtualizarUsuario(usuario), Times.Once);
        }

        [Fact]
        public async Task AtualizarUsuario_DeveRetornarFalha_QuandoIdUsuarioInvalido()
        {
            var request = new UsuarioAlteracaoRequestDto { IdUsuario = 0, Nome = "Teste", Email = "teste@email.com", IdPerfil = 1 };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _usuarioBusiness.AtualizarUsuario(request));
            Assert.Equal("O ID do usuário deve ser fornecido para atualização.", ex.Message);
        }

        [Fact]
        public async Task ExcluirUsuario_DeveRetornarSucesso_QuandoIdUsuarioValido()
        {
            int idUsuario = 1;
            _usuarioRepoMock.Setup(r => r.ExcluirUsuario(idUsuario)).Returns(Task.CompletedTask);

            await _usuarioBusiness.ExcluirUsuario(idUsuario);

            _usuarioRepoMock.Verify(r => r.ExcluirUsuario(idUsuario), Times.Once);
        }

        [Fact]
        public async Task ExcluirUsuario_DeveRetornarFalha_QuandoIdUsuarioInvalido()
        {
            int idUsuario = 0;

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _usuarioBusiness.ExcluirUsuario(idUsuario));
            Assert.Equal("O ID do usuário deve ser informado para exclusão.", ex.Message);
        }

        [Fact]
        public async Task ObterUsuarioPorId_DeveRetornarSucesso_QuandoIdUsuarioValido()
        {
            int idUsuario = 1;
            var usuario = new Usuario { IdUsuario = 1, Nome = "Teste", Email = "teste@email.com", IdPerfil = 1 };
            var response = new UsuarioCadastroResponseDto { IdUsuario = 1, Nome = "Teste", Email = "teste@email.com", IdPerfil = 1 };

            _usuarioRepoMock.Setup(r => r.ObterUsuarioPorId(idUsuario)).ReturnsAsync(usuario);
            _mapperMock.Setup(m => m.Map<UsuarioCadastroResponseDto>(usuario)).Returns(response);

            var result = await _usuarioBusiness.ObterUsuarioPorId(idUsuario);

            Assert.NotNull(result);
            Assert.Equal(1, result.IdUsuario);
        }

        [Fact]
        public async Task ObterUsuarioPorId_DeveRetornarFalha_QuandoIdUsuarioInvalido()
        {
            int idUsuario = 0;

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _usuarioBusiness.ObterUsuarioPorId(idUsuario));
            Assert.Equal("O ID do usuário deve ser informado para obter.", ex.Message);
        }

        [Fact]
        public async Task ListarUsuarios_DeveRetornarSucesso_QuandoFiltroValido()
        {
            var filtro = new UsuarioFiltroDto { Nome = "Teste" };
            var usuarios = new List<Usuario>
                {
                    new Usuario { IdUsuario = 1, Nome = "Teste", Email = "teste@email.com", IdPerfil = 1 }
                };
            var responses = new List<UsuarioCadastroResponseDto>
                {
                    new UsuarioCadastroResponseDto { IdUsuario = 1, Nome = "Teste", Email = "teste@email.com", IdPerfil = 1 }
                };

            _usuarioRepoMock.Setup(r => r.ListarUsuarios(filtro)).ReturnsAsync(usuarios);
            _mapperMock.Setup(m => m.Map<IEnumerable<UsuarioCadastroResponseDto>>(usuarios)).Returns(responses);

            var result = await _usuarioBusiness.ListarUsuarios(filtro);

            Assert.NotNull(result);
            Assert.Single(result);
        }
    }
}
