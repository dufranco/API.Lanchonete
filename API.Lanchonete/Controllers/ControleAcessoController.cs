using API.Lanchonete.Domain.DTO.Request;
using API.Lanchonete.Domain.DTO.Request.Filtro;
using API.Lanchonete.Domain.DTO.Response;
using API.Lanchonete.Domain.Interfaces.Business;
using API.Lanchonete.Domain.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace API.Lanchonete.Controllers
{
    [Route("lanchonete/api/[controller]")]
    [ApiController]
    public class ControleAcessoController(ILogger<ControleAcessoController> logger, IControleAcessoBusiness controleAcessoBusiness) : ControllerBase
    {
        private readonly ILogger<ControleAcessoController> _logger = logger;
        private readonly IControleAcessoBusiness _controleAcessoBusiness = controleAcessoBusiness;

        [HttpPost("cadastrar")]
        public async Task<ActionResult<ControleAcessoCadastroResponseDto>> CadastrarControleAcesso([FromBody][Required] ControleAcessoCadastroRequestDto controleAcesso)
        {
            try
            {
                _logger.LogInformation("Cadastro de controle de acesso iniciado.");
                var validationResult = new ControleAcessoCadastroRequestDtoValidator().Validate(controleAcesso);

                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Falha na validação do controle de acesso: {Errors}", validationResult.Errors);
                    return BadRequest(validationResult.Errors);
                }

                var result = await _controleAcessoBusiness.CadastrarControleAcesso(controleAcesso);
                _logger.LogInformation("Controle de acesso cadastrado com sucesso: {ControleAcessoId}", result.IdControle);

                return Created(string.Empty, result);
            }
            catch (DbUpdateException duEx)
            {
                _logger.LogError(duEx, "Erro ao cadastrar controle de acesso: {Message}", $"{duEx.Message}");
                return BadRequest(duEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cadastrar controle de acesso: {Message}", $"{ex.Message}{Environment.NewLine}{ex.InnerException}");
                return Problem("Ocorreu um erro ao processar a solicitação.");
            }
        }

        [HttpPut("atualizar")]
        public async Task<ActionResult> AtualizarControleAcesso([FromBody][Required] ControleAcessoAlteracaoRequestDto controleAcesso)
        {
            const string erro = "Erro ao atualizar controle de acesso: {Message}";

            try
            {
                _logger.LogInformation("Atualização de controle de acesso iniciada.");
                var validationResult = new ControleAcessoAlteracaoRequestDtoValidator().Validate(controleAcesso);

                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Falha na validação do controle de acesso: {Errors}", validationResult.Errors);
                    return BadRequest(validationResult.Errors);
                }

                await _controleAcessoBusiness.AtualizarControleAcesso(controleAcesso);
                _logger.LogInformation("Controle de acesso atualizado com sucesso.");

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
                return Problem("Ocorreu um erro ao processar a solicitação.");
            }
        }

        [HttpDelete("excluir/{idControle}")]
        public async Task<ActionResult> ExcluirControleAcesso([Required] int idControle)
        {
            try
            {
                _logger.LogInformation("Exclusão de controle de acesso iniciada.");
                await _controleAcessoBusiness.ExcluirControleAcesso(idControle);
                _logger.LogInformation("Controle de acesso excluído com sucesso.");

                return NoContent();
            }
            catch (KeyNotFoundException knfEx)
            {
                _logger.LogWarning(knfEx, "Controle de acesso não encontrado para exclusão: {Message}", knfEx.Message);
                return NotFound(knfEx.Message);
            }
            catch (InvalidOperationException invOpEx)
            {
                _logger.LogWarning(invOpEx, "Erro ao excluir controle de acesso devido a associações: {Message}", invOpEx.Message);
                return BadRequest(invOpEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir controle de acesso: {Message}", $"{ex.Message}{Environment.NewLine}{ex.InnerException}");
                return Problem("Ocorreu um erro ao processar a solicitação.");
            }
        }

        [HttpGet("obtercontroleacesso/{idControle}")]
        public async Task<ActionResult<ControleAcessoCadastroResponseDto>> ObterControleAcesso([Required] int idControle)
        {
            try
            {
                _logger.LogInformation("Obtenção de controle de acesso iniciada.");
                var controleAcesso = await _controleAcessoBusiness.ObterControleAcessoPorId(idControle);
                _logger.LogInformation("Controle de acesso obtido com sucesso: {ControleAcessoId}", controleAcesso.IdControle);

                return Ok(controleAcesso);
            }
            catch (KeyNotFoundException knfEx)
            {
                _logger.LogWarning(knfEx, "Controle de acesso não encontrado: {IdControle}", idControle);
                return NotFound(knfEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter controle de acesso: {Message}", $"{ex.Message}{Environment.NewLine}{ex.InnerException}");
                return Problem("Ocorreu um erro ao processar a solicitação.");
            }
        }

        [HttpGet("listarcontroleacessos")]
        public async Task<ActionResult<IEnumerable<ControleAcessoCadastroResponseDto>>> ListarControleAcessos(
            [FromQuery] int? idControle = null,
            [FromQuery] int? idPerfil = null,
            [FromQuery] string? nomePerfil = null,
            [FromQuery] string? nomeTela = null,
            [FromQuery] bool? permitido = null,
            [FromQuery] DateTime? dataCadastroIni = null,
            [FromQuery] DateTime? dataCadastroFim = null,
            [FromQuery] int? OrdenarPor = 0,
            [FromQuery] bool OrdemDesc = false,
            [FromQuery] int Pagina = 1,
            [FromQuery] int TamanhoPagina = 10)
        {
            try
            {
                _logger.LogInformation("Listagem de controle de acessos iniciada.");
                var controleAcessoFiltro = new ControleAcessoFiltroDto
                {
                    IdControle = idControle,
                    IdPerfil = idPerfil,
                    NomePerfil = nomePerfil,
                    NomeTela = nomeTela,
                    Permitido = permitido,
                    DataCadastroIni = dataCadastroIni,
                    DataCadastroFim = dataCadastroFim,
                    OrdenarPor = OrdenarPor,
                    OrdemDesc = OrdemDesc,
                    Pagina = Pagina,
                    TamanhoPagina = TamanhoPagina
                };
                var validationResult = new ControleAcessoFiltroDtoValidator().Validate(controleAcessoFiltro);

                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Falha na validação do controle de acesso: {Errors}", validationResult.Errors);
                    return BadRequest(validationResult.Errors);
                }

                var controleAcessos = await _controleAcessoBusiness.ListarControleAcessos(controleAcessoFiltro);

                if (controleAcessos == null || !controleAcessos.Any())
                {
                    _logger.LogInformation("Nenhum controle de acesso encontrado.");
                    return NotFound("Nenhum controle de acesso encontrado.");
                }

                _logger.LogInformation("Controle de acessos listados com sucesso.");
                return Ok(controleAcessos);
            }
            catch (KeyNotFoundException knfEx)
            {
                _logger.LogWarning(knfEx, "Nenhum controle de acesso encontrado.");
                return NotFound(knfEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar controle de acessos: {Message}", $"{ex.Message}{Environment.NewLine}{ex.InnerException}");
                return Problem("Ocorreu um erro ao processar a solicitação.");
            }
        }
    }
}
