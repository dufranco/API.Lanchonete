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
    public class ProdutoController(ILogger<ProdutoController> logger, IProdutoBusiness produtoBusiness) : ControllerBase
    {
        private readonly ILogger<ProdutoController> _logger = logger;
        private readonly IProdutoBusiness _produtoBusiness = produtoBusiness;

        [HttpPost("Cadastrar")]
        public async Task<ActionResult<ProdutoCadastroResponseDto>> CadastrarProduto([FromBody][Required] ProdutoCadastroRequestDto produto)
        {
            try
            {
                _logger.LogInformation("Cadastro de produto iniciado.");
                var validationResult = new ProdutoCadastroRequestDtoValidator().Validate(produto);

                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Falha na validação do produto: {Errors}", validationResult.Errors);
                    return BadRequest(validationResult.Errors);
                }

                var result = await _produtoBusiness.CadastrarProduto(produto);
                _logger.LogInformation("Produto cadastrado com sucesso: {ProdutoId}", result.IdProduto);

                return Created(string.Empty, result);
            }
            catch (DbUpdateException duEx)
            {
                _logger.LogError(duEx, "Erro ao cadastrar produto: {Message}", $"{duEx.Message}");
                return BadRequest(duEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cadastrar produto: {Message}", $"{ex.Message}{Environment.NewLine}{ex.InnerException}");
                return Problem("Ocorreu um erro ao processar a solicitação.");
            }
        }

        [HttpPut("Atualizar")]
        public async Task<ActionResult> AtualizarProduto([FromBody][Required] ProdutoAlteracaoRequestDto produto)
        {
            const string erro = "Erro ao atualizar produto: {Message}";

            try
            {
                _logger.LogInformation("Atualização de produto iniciada.");
                var validationResult = new ProdutoAlteracaoRequestDtoValidator().Validate(produto);

                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Falha na validação do produto: {Errors}", validationResult.Errors);
                    return BadRequest(validationResult.Errors);
                }

                await _produtoBusiness.AtualizarProduto(produto);
                _logger.LogInformation("Produto atualizado com sucesso.");

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

        [HttpDelete("Excluir/{idProduto}")]
        public async Task<ActionResult> ExcluirProduto([Required] int idProduto)
        {
            try
            {
                _logger.LogInformation("Exclusão de produto iniciada.");
                await _produtoBusiness.ExcluirProduto(idProduto);
                _logger.LogInformation("Produto excluído com sucesso.");

                return NoContent();
            }
            catch (KeyNotFoundException knfEx)
            {
                _logger.LogWarning(knfEx, "Produto não encontrado para exclusão: {Message}", knfEx.Message);
                return NotFound(knfEx.Message);
            }
            catch (InvalidOperationException invOpEx)
            {
                _logger.LogWarning(invOpEx, "Erro ao excluir produto devido a associações: {Message}", invOpEx.Message);
                return BadRequest(invOpEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir produto: {Message}", $"{ex.Message}{Environment.NewLine}{ex.InnerException}");
                return Problem("Ocorreu um erro ao processar a solicitação.");
            }
        }

        [HttpGet("ObterProduto/{idProduto}")]
        public async Task<ActionResult<ProdutoCadastroResponseDto>> ObterProduto([Required] int idProduto)
        {
            try
            {
                _logger.LogInformation("Obtenção de produto iniciada.");
                var produto = await _produtoBusiness.ObterProdutoPorId(idProduto);
                _logger.LogInformation("Produto obtido com sucesso: {ProdutoId}", produto.IdProduto);

                return Ok(produto);
            }
            catch (KeyNotFoundException knfEx)
            {
                _logger.LogWarning(knfEx, "Produto não encontrado: {IdProduto}", idProduto);
                return NotFound(knfEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter produto: {Message}", $"{ex.Message}{Environment.NewLine}{ex.InnerException}");
                return Problem("Ocorreu um erro ao processar a solicitação.");
            }
        }

        [HttpGet("ListarProdutos")]
        public async Task<ActionResult<IEnumerable<ProdutoCadastroResponseDto>>> ListarProdutos(
            [FromQuery] int? IdProduto = null,
            [FromQuery] string? Nome = null,
            [FromQuery] string? Descricao = null,
            [FromQuery] string? Tipo = null,
            [FromQuery] bool? Ativo = null,
            [FromQuery] int? OrdenarPor = 0,
            [FromQuery] bool OrdemDesc = false,
            [FromQuery] int Pagina = 1,
            [FromQuery] int TamanhoPagina = 10)
        {
            try
            {
                _logger.LogInformation("Listagem de produtos iniciada.");
                var produtoFiltro = new ProdutoFiltroDto
                {
                    IdProduto = IdProduto,
                    Nome = Nome,
                    Descricao = Descricao,
                    Tipo = Tipo,
                    Ativo = Ativo,
                    OrdenarPor = OrdenarPor,
                    OrdemDesc = OrdemDesc,
                    Pagina = Pagina,
                    TamanhoPagina = TamanhoPagina
                };
                var validationResult = new ProdutoFiltroDtoValidator().Validate(produtoFiltro);

                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Falha na validação do produto: {Errors}", validationResult.Errors);
                    return BadRequest(validationResult.Errors);
                }

                var produtos = await _produtoBusiness.ListarProdutos(produtoFiltro);

                if (produtos == null || !produtos.Any())
                {
                    _logger.LogInformation("Nenhum produto encontrado.");
                    return NotFound("Nenhum produto encontrado.");
                }

                _logger.LogInformation("Produtos listados com sucesso.");
                return Ok(produtos);
            }
            catch (KeyNotFoundException knfEx)
            {
                _logger.LogWarning(knfEx, "Nenhum produto encontrado.");
                return NotFound(knfEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar produtos: {Message}", $"{ex.Message}{Environment.NewLine}{ex.InnerException}");
                return Problem("Ocorreu um erro ao processar a solicitação.");
            }
        }
    }
}
