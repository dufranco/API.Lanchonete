using API.Lanchonete.Business.Business;
using API.Lanchonete.Domain.DTO.Request;
using API.Lanchonete.Domain.DTO.Request.Filtro;
using API.Lanchonete.Domain.DTO.Response;
using API.Lanchonete.Domain.Entities;
using API.Lanchonete.Domain.Interfaces.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;

namespace API.Lanchonete.Tests.Business
{
    public class ControleAcessoBusinessTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IControleAcessoEFRepository> _controleAcessoRepoMock;
        private readonly ControleAcessoBusiness _controleAcessoBusiness;

        public ControleAcessoBusinessTest()
        {
            _mapperMock = new Mock<IMapper>();
            _controleAcessoRepoMock = new Mock<IControleAcessoEFRepository>();
            _controleAcessoBusiness = new ControleAcessoBusiness(_mapperMock.Object, _controleAcessoRepoMock.Object);
        }

        [Fact]
        public async Task CadastrarControleAcesso_DeveRetornarSucesso_QuandoCadastroForValido()
        {
            var request = new ControleAcessoCadastroRequestDto();
            var entity = new ControleAcesso();
            var response = new ControleAcessoCadastroResponseDto();

            _mapperMock.Setup(m => m.Map<ControleAcesso>(request)).Returns(entity);
            _controleAcessoRepoMock.Setup(r => r.CadastrarControleAcesso(entity)).ReturnsAsync(entity);
            _mapperMock.Setup(m => m.Map<ControleAcessoCadastroResponseDto>(entity)).Returns(response);

            var result = await _controleAcessoBusiness.CadastrarControleAcesso(request);

            Assert.Equal(response, result);
        }

        [Fact]
        public async Task AtualizarControleAcesso_DeveRetornarSucesso_QuandoAtualizacaoForValida()
        {
            var request = new ControleAcessoAlteracaoRequestDto { IdControle = 1 };
            var entity = new ControleAcesso();

            _mapperMock.Setup(m => m.Map<ControleAcesso>(request)).Returns(entity);
            _controleAcessoRepoMock.Setup(r => r.AtualizarControleAcesso(entity)).Returns(Task.CompletedTask);

            await _controleAcessoBusiness.AtualizarControleAcesso(request);

            _controleAcessoRepoMock.Verify(r => r.AtualizarControleAcesso(entity), Times.Once);
        }

        [Fact]
        public async Task AtualizarControleAcesso_DeveRetornarFalha_QuandoIdControleForInvalido()
        {
            var request = new ControleAcessoAlteracaoRequestDto { IdControle = 0 };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _controleAcessoBusiness.AtualizarControleAcesso(request));
            Assert.Contains("ID do controle de acesso deve ser fornecido", ex.Message);
        }

        [Fact]
        public async Task ExcluirControleAcesso_DeveRetornarSucesso_QuandoIdForValido()
        {
            int id = 1;
            _controleAcessoRepoMock.Setup(r => r.ExcluirControleAcesso(id)).Returns(Task.CompletedTask);

            await _controleAcessoBusiness.ExcluirControleAcesso(id);

            _controleAcessoRepoMock.Verify(r => r.ExcluirControleAcesso(id), Times.Once);
        }

        [Fact]
        public async Task ExcluirControleAcesso_DeveRetornarFalha_QuandoIdForInvalido()
        {
            int id = 0;

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _controleAcessoBusiness.ExcluirControleAcesso(id));
            Assert.Contains("ID do controle de acesso deve ser informado", ex.Message);
        }

        [Fact]
        public async Task ObterControleAcessoPorId_DeveRetornarSucesso_QuandoIdForValido()
        {
            int id = 1;
            var entity = new ControleAcesso();
            var response = new ControleAcessoResponseDto();

            _controleAcessoRepoMock.Setup(r => r.ObterControleAcessoPorId(id)).ReturnsAsync(entity);
            _mapperMock.Setup(m => m.Map<ControleAcessoResponseDto>(entity)).Returns(response);

            var result = await _controleAcessoBusiness.ObterControleAcessoPorId(id);

            Assert.Equal(response, result);
        }

        [Fact]
        public async Task ObterControleAcessoPorId_DeveRetornarFalha_QuandoIdForInvalido()
        {
            int id = 0;

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _controleAcessoBusiness.ObterControleAcessoPorId(id));
            Assert.Contains("ID do controle de acesso deve ser informado", ex.Message);
        }

        [Fact]
        public async Task ListarControleAcessos_DeveRetornarSucesso_QuandoFiltroForValido()
        {
            var filtro = new ControleAcessoFiltroDto();
            var entities = new List<ControleAcesso> { new ControleAcesso() };
            var dtos = new List<ControleAcessoResponseDto> { new ControleAcessoResponseDto() };

            _controleAcessoRepoMock.Setup(r => r.ListarControleAcessos(filtro)).ReturnsAsync(entities);
            _mapperMock.Setup(m => m.Map<IEnumerable<ControleAcessoResponseDto>>(entities)).Returns(dtos);

            var result = await _controleAcessoBusiness.ListarControleAcessos(filtro);

            Assert.Equal(dtos, result);
        }

        [Fact]
        public async Task ListarControleAcessos_DeveRetornarFalha_QuandoRepositorioRetornarNulo()
        {
            var filtro = new ControleAcessoFiltroDto();
            _controleAcessoRepoMock.Setup(r => r.ListarControleAcessos(filtro)).ReturnsAsync((IEnumerable<ControleAcesso>)null);

            _mapperMock.Setup(m => m.Map<IEnumerable<ControleAcessoResponseDto>>(null)).Returns((IEnumerable<ControleAcessoResponseDto>)null);

            var result = await _controleAcessoBusiness.ListarControleAcessos(filtro);

            Assert.Null(result);
        }
    }
}
