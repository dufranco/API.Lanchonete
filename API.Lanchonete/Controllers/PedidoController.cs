using API.Lanchonete.Core.Utils;
using API.Lanchonete.Domain.DTO.Request;
using API.Lanchonete.Domain.DTO.Request.Filtro;
using API.Lanchonete.Domain.DTO.Response;
using API.Lanchonete.Domain.Entities;
using API.Lanchonete.Domain.Interfaces.Business;
using API.Lanchonete.Domain.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace API.Lanchonete.Controllers
{
    [Route("lanchonete/api/[controller]")]
    [ApiController]
    public class PedidoController(ILogger<PedidoController> logger, IPedidoBusiness pedidoBusiness) : ControllerBase
    {
        private readonly ILogger<PedidoController> _logger = logger;
        private readonly IPedidoBusiness _pedidoBusiness = pedidoBusiness;

        [HttpPost("cadastrar")]
        public async Task<ActionResult<PedidoCadastroResponseDto>> CadastrarPedido([FromBody][Required] PedidoCadastroRequestDto pedidoCadastro)
        {
            try
            {
                _logger.LogInformation("Cadastro de pedido iniciado.");
                var validationResult = new PedidoCadastroRequestDtoValidator().Validate(pedidoCadastro);

                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Falha na validação do pedido: {Errors}", validationResult.Errors);
                    return BadRequest(validationResult.Errors);
                }

                var result = await _pedidoBusiness.CadastrarPedido(pedidoCadastro);
                _logger.LogInformation("Pedido cadastrado com sucesso: {PedidoId}", result.IdPedido);

                return Created(string.Empty, result);
            }
            catch (DbUpdateException duEx)
            {
                _logger.LogError(duEx, "Erro ao cadastrar pedido: {Message}", $"{duEx.Message}");
                return BadRequest(duEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cadastrar pedido: {Message}", $"{ex.Message}{Environment.NewLine}{ex.InnerException}");
                return Problem(MensagensPadrao.ErroGenerico);
            }
        }

        [HttpPut("atualizar")]
        public async Task<ActionResult> AtualizarPedido([FromBody][Required] PedidoAlteracaoRequestDto pedidoAlteracao)
        {
            const string erro = "Erro ao atualizar pedido: {Message}";

            try
            {
                _logger.LogInformation("Atualização de pedido iniciada.");
                var validationResult = new PedidoAlteracaoRequestDtoValidator().Validate(pedidoAlteracao);

                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Falha na validação do pedido: {Errors}", validationResult.Errors);
                    return BadRequest(validationResult.Errors);
                }

                await _pedidoBusiness.AtualizarPedido(pedidoAlteracao);
                _logger.LogInformation("Pedido atualizado com sucesso.");

                return NoContent();
            }
            catch (ArgumentException argEx)
            {
                _logger.LogWarning(argEx, erro, argEx.Message);
                return BadRequest(argEx.Message);
            }
            catch (DbUpdateException duEx)
            {
                _logger.LogWarning(duEx, erro, $"{duEx.Message}");
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

        [HttpPost("{idPedido}/item")]
        public async Task<ActionResult<ItemPedidoCadastroResponseDto>> IncluirItemPedido([Required] int idPedido, [Required][FromBody] ItemPedidoRequestDto itemPedido)
        {
            try
            {
                _logger.LogInformation("Inclusão de item de pedido iniciada.");
                itemPedido.IdPedido = idPedido;
                var result = await _pedidoBusiness.IncluirItemPedido(itemPedido);
                _logger.LogInformation("Pedido excluído com sucesso.");

                return Created(string.Empty, result);
            }
            catch (InvalidOperationException knfEx)
            {
                _logger.LogWarning(knfEx, knfEx.Message);
                return NotFound(knfEx.Message);
            }
            catch (KeyNotFoundException knfEx)
            {
                _logger.LogWarning(knfEx, "Pedido não encontrado para inclusão: {Message}", knfEx.Message);
                return NotFound(knfEx.Message);
            }
            catch (DbUpdateException duEx)
            {
                _logger.LogWarning(duEx, "Pedido não encontrado para inclusão: {Message}", $"{duEx.Message}");
                return BadRequest(duEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir pedido: {Message}", $"{ex.Message}{Environment.NewLine}{ex.InnerException}");
                return Problem(MensagensPadrao.ErroGenerico);
            }
        }

        [HttpDelete("{idPedido}")]
        public async Task<ActionResult> ExcluirPedido([Required] int idPedido)
        {
            try
            {
                _logger.LogInformation("Exclusão de pedido iniciada.");
                await _pedidoBusiness.ExcluirPedido(idPedido);
                _logger.LogInformation("Pedido excluído com sucesso.");

                return NoContent();
            }
            catch (KeyNotFoundException knfEx)
            {
                _logger.LogWarning(knfEx, "Pedido não encontrado para exclusão: {Message}", knfEx.Message);
                return NotFound(knfEx.Message);
            }
            catch (InvalidOperationException invOpEx)
            {
                _logger.LogWarning(invOpEx, "Erro ao excluir pedido devido a associações: {Message}", invOpEx.Message);
                return BadRequest(invOpEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir pedido: {Message}", $"{ex.Message}{Environment.NewLine}{ex.InnerException}");
                return Problem(MensagensPadrao.ErroGenerico);
            }
        }

        [HttpDelete("{idPedido}/item/{idItemPedido}")]
        public async Task<ActionResult> ExcluirItemPedido([Required] int idPedido, [Required] int idItemPedido)
        {
            try
            {
                _logger.LogInformation("Exclusão de item de pedido iniciada.");
                await _pedidoBusiness.ExcluirItemPedido(idItemPedido, idPedido);
                _logger.LogInformation("Pedido excluído com sucesso.");

                return NoContent();
            }
            catch (KeyNotFoundException knfEx)
            {
                _logger.LogWarning(knfEx, "Pedido não encontrado para exclusão: {Message}", knfEx.Message);
                return NotFound(knfEx.Message);
            }
            catch (InvalidOperationException invOpEx)
            {
                _logger.LogWarning(invOpEx, "Erro ao excluir pedido devido a associações: {Message}", invOpEx.Message);
                return BadRequest(invOpEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir pedido: {Message}", $"{ex.Message}{Environment.NewLine}{ex.InnerException}");
                return Problem(MensagensPadrao.ErroGenerico);
            }
        }

        [HttpGet("{idPedido}")]
        public async Task<ActionResult<PedidoCadastroResponseDto>> ObterPedido([Required] int idPedido)
        {
            try
            {
                _logger.LogInformation("Obtenção de pedido iniciada.");
                var pedido = await _pedidoBusiness.ObterPedidoPorId(idPedido);
                _logger.LogInformation("Pedido obtido com sucesso: {PedidoId}", pedido.IdPedido);

                return Ok(pedido);
            }
            catch (KeyNotFoundException knfEx)
            {
                _logger.LogWarning(knfEx, "Pedido não encontrado: {IdPedido}", idPedido);
                return NotFound(knfEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter pedido: {Message}", $"{ex.Message}{Environment.NewLine}{ex.InnerException}");
                return Problem(MensagensPadrao.ErroGenerico);
            }
        }

        [HttpGet("listarpedidos")]
        public async Task<ActionResult<IEnumerable<PedidoResponseDto>>> ListarPedidos(
            [FromQuery] int? idPedido = null,
            [FromQuery] DateTime? dataCadastroIni = null,
            [FromQuery] DateTime? dataCadastroFim = null,
            [FromQuery] string? status = null,
            [FromQuery] int? idProduto = null,
            [FromQuery] int? ordenarPor = 0,
            [FromQuery] bool ordemDesc = false,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanhoPagina = 10)
        {
            try
            {
                _logger.LogInformation("Listagem de pedidos iniciada.");
                var pedidoFiltro = new PedidoFiltroDto
                {
                    IdPedido = idPedido,
                    DataCadastroIni = dataCadastroIni,
                    DataCadastroFim = dataCadastroFim,
                    IdProduto = idProduto,
                    Status = status,
                    OrdenarPor = ordenarPor,
                    OrdemDesc = ordemDesc,
                    Pagina = pagina,
                    TamanhoPagina = tamanhoPagina
                };
                var validationResult = new PedidoFiltroDtoValidator().Validate(pedidoFiltro);

                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Falha na requisição: {Errors}", validationResult.Errors);
                    return BadRequest(validationResult.Errors);
                }

                var pedidos = await _pedidoBusiness.ListarPedidos(pedidoFiltro);
                _logger.LogInformation("Pedidos listados com sucesso.");

                return Ok(pedidos);
            }
            catch (KeyNotFoundException knfEx)
            {
                _logger.LogWarning(knfEx, "Nenhum pedido encontrado.");
                return NotFound(knfEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar pedidos: {Message}", $"{ex.Message}{Environment.NewLine}{ex.InnerException}");
                return Problem(MensagensPadrao.ErroGenerico);
            }
        }
    }
}
