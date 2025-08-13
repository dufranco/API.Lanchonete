using API.Lanchonete.Domain.DTO.Request.Filtro;
using FluentValidation;

namespace API.Lanchonete.Domain.Validators
{
    public class ProdutoFiltroDtoValidator : AbstractValidator<ProdutoFiltroDto>
    {
        public ProdutoFiltroDtoValidator()
        {
            RuleFor(x => x.IdProduto)
                .GreaterThan(0)
                .When(x => x.IdProduto.HasValue)
                .WithMessage("O identificador do produto deve ser maior que zero.");

            RuleFor(x => x.Nome)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.Nome))
                .WithMessage("O nome deve ter no máximo 100 caracteres.");

            RuleFor(x => x.Descricao)
                .MaximumLength(255)
                .When(x => !string.IsNullOrWhiteSpace(x.Descricao))
                .WithMessage("A descrição deve ter no máximo 255 caracteres.");

            RuleFor(x => x.Tipo)
                .MaximumLength(50)
                .When(x => !string.IsNullOrWhiteSpace(x.Tipo))
                .WithMessage("O tipo deve ter no máximo 50 caracteres.");

            RuleFor(x => x.Ativo)
                .NotNull()
                .When(x => x.Ativo.HasValue)
                .WithMessage("O filtro ativo deve ser informado como verdadeiro ou falso.");

            RuleFor(x => x.Pagina)
                .GreaterThan(0)
                .WithMessage("A página deve ser maior que zero.");

            RuleFor(x => x.TamanhoPagina)
                .InclusiveBetween(1, 100)
                .WithMessage("O tamanho da página deve estar entre 1 e 100.");
        }
    }
}
