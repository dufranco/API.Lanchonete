using API.Lanchonete.Controllers;
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
    public class UsuarioControllerTest
    {
        private readonly Mock<ILogger<UsuarioController>> _loggerUsuarioControllerMock;
        private readonly Mock<IUsuarioBusiness> _usuarioBusinessMock;
        private readonly UsuarioController _controller;

        public UsuarioControllerTest()
        {
            _loggerUsuarioControllerMock = new Mock<ILogger<UsuarioController>>();
            _usuarioBusinessMock = new Mock<IUsuarioBusiness>();
            _controller = new UsuarioController(_loggerUsuarioControllerMock.Object, _usuarioBusinessMock.Object);
        }
        [Fact]
        public async Task CadastrarUsuario_DeveRetornarCreatedResult_QuandoUsuarioValido()
        {
            var request = new UsuarioCadastroRequestDto
            {
                Nome = "Teste",
                Email = "teste@email.com",
                IdPerfil = 1,
                Senha = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("senha123"))
            };
            var response = new UsuarioCadastroResponseDto
            {
                IdUsuario = 1,
                Nome = request.Nome,
                Email = request.Email,
                IdPerfil = request.IdPerfil
            };

            _usuarioBusinessMock.Setup(x => x.CadastrarUsuario(It.IsAny<UsuarioCadastroRequestDto>()))
                .ReturnsAsync(response);

            var result = await _controller.CadastrarUsuario(request);

            var createdResult = Assert.IsType<CreatedResult>(result.Result);
            Assert.Equal(response, createdResult.Value);
        }

        [Fact]
        public async Task CadastrarUsuario_DeveRetornarBadRequest_QuandoRequestInvalido()
        {
            var request = new UsuarioCadastroRequestDto
            {
                Nome = "",
                Email = "",
                IdPerfil = 0,
                Senha = null
            };

            var result = await _controller.CadastrarUsuario(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(badRequest.Value);
        }

        [Fact]
        public async Task CadastrarUsuario_DeveRetornarBadRequest_QuandoDbUpdateException()
        {
            var request = new UsuarioCadastroRequestDto
            {
                Nome = "Teste",
                Email = "teste@email.com",
                IdPerfil = 1,
                Senha = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("senha123"))
            };

            _usuarioBusinessMock.Setup(x => x.CadastrarUsuario(It.IsAny<UsuarioCadastroRequestDto>()))
                .ThrowsAsync(new Microsoft.EntityFrameworkCore.DbUpdateException("Erro DB"));

            var result = await _controller.CadastrarUsuario(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Erro DB", badRequest.Value);
        }

        [Fact]
        public async Task CadastrarUsuario_DeveRetornarErroInterno_QuandoException()
        {
            var request = new UsuarioCadastroRequestDto
            {
                Nome = "Teste",
                Email = "teste@email.com",
                IdPerfil = 1,
                Senha = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("senha123"))
            };

            _usuarioBusinessMock.Setup(x => x.CadastrarUsuario(It.IsAny<UsuarioCadastroRequestDto>()))
                .ThrowsAsync(new Exception("Erro genérico"));

            var result = await _controller.CadastrarUsuario(request);

            var problemResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, problemResult.StatusCode);
        }

        [Fact]
        public async Task AtualizarUsuario_DeveRetornarNoContent_QuandoUsuarioValido()
        {
            var request = new UsuarioAlteracaoRequestDto
            {
                IdUsuario = 1,
                Nome = "Teste",
                Email = "teste@email.com",
                IdPerfil = 1,
                Senha = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("senha123"))
            };

            _usuarioBusinessMock.Setup(x => x.AtualizarUsuario(It.IsAny<UsuarioAlteracaoRequestDto>()))
                .Returns(Task.CompletedTask);

            var result = await _controller.AtualizarUsuario(request);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task AtualizarUsuario_DeveRetornarBadRequest_QuandoRequestInvalido()
        {
            var request = new UsuarioAlteracaoRequestDto
            {
                IdUsuario = 0,
                Nome = "",
                Email = "",
                IdPerfil = 0,
                Senha = null
            };

            var result = await _controller.AtualizarUsuario(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequest.Value);
        }

        [Fact]
        public async Task AtualizarUsuario_DeveRetornarBadRequest_QuandoArgumentException()
        {
            var request = new UsuarioAlteracaoRequestDto
            {
                IdUsuario = 1,
                Nome = "Teste",
                Email = "teste@email.com",
                IdPerfil = 1,
                Senha = null
            };

            _usuarioBusinessMock.Setup(x => x.AtualizarUsuario(It.IsAny<UsuarioAlteracaoRequestDto>()))
                .ThrowsAsync(new ArgumentException("Argumento inválido"));

            var result = await _controller.AtualizarUsuario(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Argumento inválido", badRequest.Value);
        }

        [Fact]
        public async Task AtualizarUsuario_DeveRetornarBadRequest_QuandoDbUpdateException()
        {
            var request = new UsuarioAlteracaoRequestDto
            {
                IdUsuario = 1,
                Nome = "Teste",
                Email = "teste@email.com",
                IdPerfil = 1,
                Senha = null
            };

            _usuarioBusinessMock.Setup(x => x.AtualizarUsuario(It.IsAny<UsuarioAlteracaoRequestDto>()))
                .ThrowsAsync(new Microsoft.EntityFrameworkCore.DbUpdateException("Erro DB"));

            var result = await _controller.AtualizarUsuario(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Erro DB", badRequest.Value);
        }

        [Fact]
        public async Task AtualizarUsuario_DeveRetornarNotFound_QuandoKeyNotFoundException()
        {
            var request = new UsuarioAlteracaoRequestDto
            {
                IdUsuario = 1,
                Nome = "Teste",
                Email = "teste@email.com",
                IdPerfil = 1,
                Senha = null
            };

            _usuarioBusinessMock.Setup(x => x.AtualizarUsuario(It.IsAny<UsuarioAlteracaoRequestDto>()))
                .ThrowsAsync(new KeyNotFoundException("Não encontrado"));

            var result = await _controller.AtualizarUsuario(request);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Não encontrado", notFound.Value);
        }

        [Fact]
        public async Task AtualizarUsuario_DeveRetornarErroInterno_QuandoException()
        {
            var request = new UsuarioAlteracaoRequestDto
            {
                IdUsuario = 1,
                Nome = "Teste",
                Email = "teste@email.com",
                IdPerfil = 1,
                Senha = null
            };

            _usuarioBusinessMock.Setup(x => x.AtualizarUsuario(It.IsAny<UsuarioAlteracaoRequestDto>()))
                .ThrowsAsync(new Exception("Erro genérico"));

            var result = await _controller.AtualizarUsuario(request);

            var problemResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, problemResult.StatusCode);
        }

        [Fact]
        public async Task ExcluirUsuario_DeveRetornarNoContent_QuandoSucesso()
        {
            _usuarioBusinessMock.Setup(x => x.ExcluirUsuario(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            var result = await _controller.ExcluirUsuario(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task ExcluirUsuario_DeveRetornarNotFound_QuandoKeyNotFoundException()
        {
            _usuarioBusinessMock.Setup(x => x.ExcluirUsuario(It.IsAny<int>()))
                .ThrowsAsync(new KeyNotFoundException("Não encontrado"));

            var result = await _controller.ExcluirUsuario(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Não encontrado", notFound.Value);
        }

        [Fact]
        public async Task ExcluirUsuario_DeveRetornarBadRequest_QuandoInvalidOperationException()
        {
            _usuarioBusinessMock.Setup(x => x.ExcluirUsuario(It.IsAny<int>()))
                .ThrowsAsync(new InvalidOperationException("Associações"));

            var result = await _controller.ExcluirUsuario(1);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Associações", badRequest.Value);
        }

        [Fact]
        public async Task ExcluirUsuario_DeveRetornarErroInterno_QuandoException()
        {
            _usuarioBusinessMock.Setup(x => x.ExcluirUsuario(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Erro genérico"));

            var result = await _controller.ExcluirUsuario(1);

            var problemResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, problemResult.StatusCode);
        }

        [Fact]
        public async Task ObterUsuario_DeveRetornarOk_QuandoUsuarioExiste()
        {
            var response = new UsuarioCadastroResponseDto
            {
                IdUsuario = 1,
                Nome = "Teste",
                Email = "teste@email.com",
                IdPerfil = 1
            };

            _usuarioBusinessMock.Setup(x => x.ObterUsuarioPorId(It.IsAny<int>()))
                .ReturnsAsync(response);

            var result = await _controller.ObterUsuario(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(response, okResult.Value);
        }

        [Fact]
        public async Task ObterUsuario_DeveRetornarNotFound_QuandoKeyNotFoundException()
        {
            _usuarioBusinessMock.Setup(x => x.ObterUsuarioPorId(It.IsAny<int>()))
                .ThrowsAsync(new KeyNotFoundException("Não encontrado"));

            var result = await _controller.ObterUsuario(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Não encontrado", notFound.Value);
        }

        [Fact]
        public async Task ObterUsuario_DeveRetornarErroInterno_QuandoException()
        {
            _usuarioBusinessMock.Setup(x => x.ObterUsuarioPorId(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Erro genérico"));

            var result = await _controller.ObterUsuario(1);

            var problemResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, problemResult.StatusCode);
        }

        [Fact]
        public async Task ListarUsuarios_DeveRetornarOk_QuandoUsuariosExistem()
        {
            var response = new List<UsuarioCadastroResponseDto>
                {
                    new UsuarioCadastroResponseDto
                    {
                        IdUsuario = 1,
                        Nome = "Teste",
                        Email = "teste@email.com",
                        IdPerfil = 1
                    }
                };

            _usuarioBusinessMock.Setup(x => x.ListarUsuarios(It.IsAny<UsuarioFiltroDto>()))
                .ReturnsAsync(response);

            var result = await _controller.ListarUsuarios();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(response, okResult.Value);
        }

        [Fact]
        public async Task ListarUsuarios_DeveRetornarBadRequest_QuandoFiltroInvalido()
        {
            var result = await _controller.ListarUsuarios(nome: "", email: "", ordenarPor: 0, ordemDesc: false, pagina: 0, tamanhoPagina: 0);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(badRequest.Value);
        }

        [Fact]
        public async Task ListarUsuarios_DeveRetornarNotFound_QuandoKeyNotFoundException()
        {
            _usuarioBusinessMock.Setup(x => x.ListarUsuarios(It.IsAny<UsuarioFiltroDto>()))
                .ThrowsAsync(new KeyNotFoundException("Nenhum usuário encontrado"));

            var result = await _controller.ListarUsuarios();

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Nenhum usuário encontrado", notFound.Value);
        }

        [Fact]
        public async Task ListarUsuarios_DeveRetornarErroInterno_QuandoException()
        {
            _usuarioBusinessMock.Setup(x => x.ListarUsuarios(It.IsAny<UsuarioFiltroDto>()))
                .ThrowsAsync(new Exception("Erro genérico"));

            var result = await _controller.ListarUsuarios();

            var problemResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, problemResult.StatusCode);
        }
    }
}
