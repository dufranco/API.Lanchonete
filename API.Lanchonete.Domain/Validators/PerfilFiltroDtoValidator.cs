using API.Lanchonete.Domain.DTO.Request.Filtro;
using FluentValidation;

namespace API.Lanchonete.Domain.Validators
{
    public class PerfilFiltroDtoValidator : AbstractValidator<PerfilFiltroDto>
    {
        public PerfilFiltroDtoValidator()
        {
            RuleFor(x => x.Nome)
                .MaximumLength(50)
                .WithMessage("O filtro Nome deve ter no máximo 50 caracteres.");

            RuleFor(x => x.Descricao)
                .MaximumLength(100)
                .WithMessage("O filtro Descrição deve ter no máximo 100 caracteres.");

            RuleFor(x => x.OrdenarPor)
                .InclusiveBetween(0, 2)
                .When(x => x.OrdenarPor.HasValue)
                .WithMessage("A ordenação deve estar entre 0 e 2.");

            RuleFor(x => x.Pagina)
                .GreaterThan(0)
                .WithMessage("A página deve ser maior que zero.");

            RuleFor(x => x.TamanhoPagina)
                .InclusiveBetween(1, 100)
                .WithMessage("O tamanho da página deve ser maior que zero.");
        }
    }
}
