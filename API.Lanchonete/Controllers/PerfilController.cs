using API.Lanchonete.Domain.DTO;
using API.Lanchonete.Domain.Entities;
using API.Lanchonete.Domain.Interfaces.Business;
using API.Lanchonete.Domain.Validation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Lanchonete.Controllers
{
    [Route("lanchonete/api/[controller]")]
    [ApiController]
    public class PerfilController : ControllerBase
    {
        private readonly ILogger<PerfilController> _logger;
        private readonly IPerfilBusiness _perfilBusiness;

        public PerfilController(ILogger<PerfilController> logger, IPerfilBusiness perfilBusiness)
        {
            _logger = logger;
            _perfilBusiness = perfilBusiness;
        }

        [HttpPost("Cadastro")]
        public async Task<ActionResult<Perfil>> CadastroPerfil([FromBody][Required] PerfilDto perfil)
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

                return Created(string.Empty, await _perfilBusiness.CadastrarPerfil(perfil));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cadastrar perfil: {Message}", $"{ex.Message}{Environment.NewLine}{ex.InnerException}");
                return Problem("Ocorreu um erro ao processar a solicitação.");
            }
        }
    }
}
