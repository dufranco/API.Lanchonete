using API.Lanchonete.Core.Utils;
using API.Lanchonete.Domain.DTO;
using API.Lanchonete.Domain.DTO.Request;
using API.Lanchonete.Domain.DTO.Response;
using API.Lanchonete.Domain.Interfaces.Business;
using API.Lanchonete.Domain.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace API.Lanchonete.Controllers
{
    [Route("lanchonete/api/[controller]")]
    [ApiController]
    public class UsuarioController(ILogger<UsuarioController> logger, IUsuarioBusiness usuarioBusiness) : ControllerBase
    {
        private readonly ILogger<UsuarioController> _logger = logger;
        private readonly IUsuarioBusiness _usuarioBusiness = usuarioBusiness;

        [HttpPost("Cadastrar")]
        public async Task<ActionResult<UsuarioCadastroResponseDto>> CadastrarUsuario([FromBody][Required] UsuarioCadastroRequestDto usuarioCadastro)
        {
            try
            {
                _logger.LogInformation("Cadastro de usuário iniciado.");
                var validationResult = new UsuarioCadastroRequestDtoValidator().Validate(usuarioCadastro);

                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Falha na validação do usuário: {Errors}", validationResult.Errors);
                    return BadRequest(validationResult.Errors);
                }

                usuarioCadastro.SenhaCriptografada = Encoding.UTF8.GetString(Convert.FromBase64String(usuarioCadastro.Senha!)).ToSecureString();
                usuarioCadastro.Senha = null;

                var result = await _usuarioBusiness.CadastrarUsuario(usuarioCadastro);
                _logger.LogInformation("Usuário cadastrado com sucesso: {UsuarioId}", result.IdUsuario);

                return Created(string.Empty, result);
            }
            catch (DbUpdateException duEx)
            {
                _logger.LogError(duEx, "Erro ao cadastrar usuário: {Message}", $"{duEx.Message}");
                return BadRequest(duEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cadastrar usuário: {Message}", $"{ex.Message}{Environment.NewLine}{ex.InnerException}");
                return Problem(MensagensPadrao.ErroGenerico);
            }
        }

        [HttpPut("Atualizar")]
        public async Task<ActionResult> AtualizarUsuario([FromBody][Required] UsuarioAlteracaoRequestDto usuarioAlteracao)
        {
            const string erro = "Erro ao atualizar usuário: {Message}";

            try
            {
                _logger.LogInformation("Atualização de usuário iniciada.");
                var validationResult = new UsuarioAlteracaoRequestDtoValidator().Validate(usuarioAlteracao);

                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Falha na validação do usuário: {Errors}", validationResult.Errors);
                    return BadRequest(validationResult.Errors);
                }

                if (usuarioAlteracao.Senha != null)
                {
                    usuarioAlteracao.SenhaCriptografada = Encoding.UTF8.GetString(Convert.FromBase64String(usuarioAlteracao.Senha!)).ToSecureString();
                    usuarioAlteracao.Senha = null;
                }

                await _usuarioBusiness.AtualizarUsuario(usuarioAlteracao);
                _logger.LogInformation("Usuário atualizado com sucesso.");

                return NoContent();
            }
            catch (ArgumentException argEx)
            {
                _logger.LogWarning(argEx, erro, argEx.Message);
                return BadRequest(argEx.Message);
            }
            catch (DbUpdateException duEx)
            {
                _logger.LogError(duEx, erro, $"{duEx.Message}");
                return BadRequest(duEx.Message);
            }
            catch (KeyNotFoundException knfEx)
            {
                _logger.LogWarning(knfEx, erro, knfEx.Message);
                return NotFound(knfEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, erro, $"{ex.Message}{Environment.NewLine}{ex.InnerException}");
                return Problem(MensagensPadrao.ErroGenerico);
            }
        }

        [HttpDelete("{idUsuario}")]
        public async Task<ActionResult> ExcluirUsuario([Required] int idUsuario)
        {
            try
            {
                _logger.LogInformation("Exclusão de usuário iniciada.");
                await _usuarioBusiness.ExcluirUsuario(idUsuario);
                _logger.LogInformation("Usuário excluído com sucesso.");

                return NoContent();
            }
            catch (KeyNotFoundException knfEx)
            {
                _logger.LogWarning(knfEx, "Usuário não encontrado para exclusão: {Message}", knfEx.Message);
                return NotFound(knfEx.Message);
            }
            catch (InvalidOperationException invOpEx)
            {
                _logger.LogWarning(invOpEx, "Erro ao excluir usuário devido a associações: {Message}", invOpEx.Message);
                return BadRequest(invOpEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir usuário: {Message}", $"{ex.Message}{Environment.NewLine}{ex.InnerException}");
                return Problem(MensagensPadrao.ErroGenerico);
            }
        }

        [HttpGet("{idUsuario}")]
        public async Task<ActionResult<UsuarioCadastroResponseDto>> ObterUsuario([Required] int idUsuario)
        {
            try
            {
                _logger.LogInformation("Obtenção de usuário iniciada.");
                var usuario = await _usuarioBusiness.ObterUsuarioPorId(idUsuario);
                _logger.LogInformation("Usuário obtido com sucesso: {UsuarioId}", usuario.IdUsuario);

                return Ok(usuario);
            }
            catch (KeyNotFoundException knfEx)
            {
                _logger.LogWarning(knfEx, "Usuário não encontrado: {IdUsuario}", idUsuario);
                return NotFound(knfEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter usuário: {Message}", $"{ex.Message}{Environment.NewLine}{ex.InnerException}");
                return Problem(MensagensPadrao.ErroGenerico);
            }
        }

        [HttpGet("ListarUsuarios")]
        public async Task<ActionResult<IEnumerable<UsuarioCadastroResponseDto>>> ListarUsuarios(
            [FromQuery] string? nome = null,
            [FromQuery] string? email = null,
            [FromQuery] int? ordenarPor = 0,
            [FromQuery] bool ordemDesc = false,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanhoPagina = 10)
        {
            try
            {
                _logger.LogInformation("Listagem de usuarios iniciada.");
                var usuarioFiltro = new UsuarioFiltroDto
                {
                    Nome = nome,
                    Email = email,
                    OrdenarPor = ordenarPor,
                    OrdemDesc = ordemDesc,
                    Pagina = pagina,
                    TamanhoPagina = tamanhoPagina
                };
                var validationResult = new UsuarioFiltroDtoValidator().Validate(usuarioFiltro);

                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Falha na requisição: {Errors}", validationResult.Errors);
                    return BadRequest(validationResult.Errors);
                }

                var usuarios = await _usuarioBusiness.ListarUsuarios(usuarioFiltro);
                _logger.LogInformation("Usuários listados com sucesso.");

                return Ok(usuarios);
            }
            catch (KeyNotFoundException knfEx)
            {
                _logger.LogWarning(knfEx, "Nenhum usuário encontrado.");
                return NotFound(knfEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar usuários: {Message}", $"{ex.Message}{Environment.NewLine}{ex.InnerException}");
                return Problem(MensagensPadrao.ErroGenerico);
            }
        }
    }
}
