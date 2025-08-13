using API.Lanchonete.Domain.DTO;
using API.Lanchonete.Domain.DTO.Request.Filtro;
using API.Lanchonete.Domain.Interfaces.Business;
using API.Lanchonete.Domain.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace API.Lanchonete.Controllers
{
    [Route("lanchonete/api/[controller]")]
    [ApiController]
    public class PerfilController(ILogger<PerfilController> logger, IPerfilBusiness perfilBusiness) : ControllerBase
    {
        private readonly ILogger<PerfilController> _logger = logger;
        private readonly IPerfilBusiness _perfilBusiness = perfilBusiness;

        [HttpPost("Cadastrar")]
        public async Task<ActionResult<PerfilDto>> CadastrarPerfil([FromBody][Required] PerfilDto perfil)
        {
            try
            {
                _logger.LogInformation("Cadastro de perfil iniciado.");
                var validationResult = new PerfilDtoValidator().Validate(perfil);

                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Falha na validação do perfil: {Errors}", validationResult.Errors);
                    return BadRequest(validationResult.Errors);
                }

                var result = await _perfilBusiness.CadastrarPerfil(perfil);
                _logger.LogInformation("Perfil cadastrado com sucesso: {PerfilId}", result.IdPerfil);

                return Created(string.Empty, result);
            }
            catch (DbUpdateException duEx)
            {
                _logger.LogError(duEx, "Erro ao cadastrar perfil: {Message}", $"{duEx.Message}");
                return BadRequest(duEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cadastrar perfil: {Message}", $"{ex.Message}{Environment.NewLine}{ex.InnerException}");
                return Problem("Ocorreu um erro ao processar a solicitação.");
            }
        }

        [HttpPut("Atualizar")]
        public async Task<ActionResult> AtualizarPerfil([FromBody][Required] PerfilDto perfil)
        {
            const string erro = "Erro ao atualizar perfil: {Message}";

            try
            {
                _logger.LogInformation("Atualização de perfil iniciada.");
                var validationResult = new PerfilDtoValidator().Validate(perfil);

                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Falha na validação do perfil: {Errors}", validationResult.Errors);
                    return BadRequest(validationResult.Errors);
                }

                await _perfilBusiness.AtualizarPerfil(perfil);
                _logger.LogInformation("Perfil atualizado com sucesso.");

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

        [HttpDelete("Excluir/{idPerfil}")]
        public async Task<ActionResult> ExcluirPerfil([Required] int idPerfil)
        {
            try
            {
                _logger.LogInformation("Exclusão de perfil iniciada.");
                await _perfilBusiness.ExcluirPerfil(idPerfil);
                _logger.LogInformation("Perfil excluído com sucesso.");

                return NoContent();
            }
            catch (KeyNotFoundException knfEx)
            {
                _logger.LogWarning(knfEx, "Perfil não encontrado para exclusão: {Message}", knfEx.Message);
                return NotFound(knfEx.Message);
            }
            catch (InvalidOperationException invOpEx)
            {
                _logger.LogWarning(invOpEx, "Erro ao excluir perfil devido a associações: {Message}", invOpEx.Message);
                return BadRequest(invOpEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir perfil: {Message}", $"{ex.Message}{Environment.NewLine}{ex.InnerException}");
                return Problem("Ocorreu um erro ao processar a solicitação.");
            }
        }

        [HttpGet("ObterPerfil/{idPerfil}")]
        public async Task<ActionResult<PerfilDto>> ObterPerfil([Required] int idPerfil)
        {
            try
            {
                _logger.LogInformation("Obtenção de perfil iniciada.");
                var perfil = await _perfilBusiness.ObterPerfilPorId(idPerfil);
                _logger.LogInformation("Perfil obtido com sucesso: {PerfilId}", perfil.IdPerfil);

                return Ok(perfil);
            }
            catch (KeyNotFoundException knfEx)
            {
                _logger.LogWarning(knfEx, "Perfil não encontrado: {IdPerfil}", idPerfil);
                return NotFound(knfEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter perfil: {Message}", $"{ex.Message}{Environment.NewLine}{ex.InnerException}");
                return Problem("Ocorreu um erro ao processar a solicitação.");
            }
        }

        [HttpGet("ListarPerfis")]
        public async Task<ActionResult<IEnumerable<PerfilDto>>> ListarPerfis(
            [FromQuery] string? filtroNome = null,
            [FromQuery] string? filtroDescricao = null,
            [FromQuery] int? ordenarPor = 0,
            [FromQuery] bool ordemDesc = false,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanhoPagina = 10)
        {
            try
            {
                _logger.LogInformation("Listagem de perfis iniciada.");
                var perfilFiltro = new PerfilFiltroDto
                {
                    Nome = filtroNome,
                    Descricao = filtroDescricao,
                    OrdenarPor = ordenarPor,
                    OrdemDesc = ordemDesc,
                    Pagina = pagina,
                    TamanhoPagina = tamanhoPagina
                };
                var validationResult = new PerfilFiltroDtoValidator().Validate(perfilFiltro);

                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Falha na validação do perfil: {Errors}", validationResult.Errors);
                    return BadRequest(validationResult.Errors);
                }

                var perfis = await _perfilBusiness.ListarPerfis(perfilFiltro);

                if (perfis == null || !perfis.Any())
                {
                    _logger.LogInformation("Nenhum perfil encontrado.");
                    return NotFound("Nenhum perfil encontrado.");
                }

                _logger.LogInformation("Perfis listados com sucesso.");
                return Ok(perfis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar perfis: {Message}", $"{ex.Message}{Environment.NewLine}{ex.InnerException}");
                return Problem("Ocorreu um erro ao processar a solicitação.");
            }
        }
    }
}
