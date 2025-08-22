using API.Lanchonete.Domain.DTO.Request.Filtro;
using FluentValidation;

namespace API.Lanchonete.Domain.Validators
{
    public class ControleAcessoFiltroDtoValidator : AbstractValidator<ControleAcessoFiltroDto>
    {
        public ControleAcessoFiltroDtoValidator()
        {
            RuleFor(x => x.IdControle)
                .GreaterThan(0)
                .When(x => x.IdControle.HasValue)
                .WithMessage("O identificador do controle de acesso deve ser maior que zero.");

            RuleFor(x => x.IdPerfil)
                .GreaterThan(0)
                .When(x => x.IdPerfil.HasValue)
                .WithMessage("O identificador do perfil deve ser maior que zero.");

            RuleFor(x => x.NomePerfil)
                .MaximumLength(50)
                .When(x => !string.IsNullOrWhiteSpace(x.NomePerfil))
                .WithMessage("O nome do perfil deve ter no máximo 50 caracteres.");

            RuleFor(x => x.NomeTela)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.NomeTela))
                .WithMessage("O nome da tela deve ter no máximo 100 caracteres.");

            RuleFor(x => x.DataCadastroIni)
                .LessThanOrEqualTo(x => x.DataCadastroFim)
                .When(x => x.DataCadastroIni.HasValue && x.DataCadastroFim.HasValue)
                .WithMessage("A data de cadastro inicío deve ser menor ou igual a data de cadastro fim.");

            RuleFor(x => x.Pagina)
                .GreaterThan(0)
                .WithMessage("A página deve ser maior que zero.");

            RuleFor(x => x.TamanhoPagina)
                .GreaterThan(0)
                .WithMessage("O tamanho da página deve ser maior que zero.");

            RuleFor(x => x.OrdenarPor)
                .InclusiveBetween(0, 5).When(x => x.OrdenarPor.HasValue)
                .WithMessage("A ordenação deve estar entre 0 e 5.");
        }
    }
}
