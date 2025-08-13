using API.Lanchonete.Domain.DTO.Request;
using FluentValidation;

namespace API.Lanchonete.Domain.Validators
{
    public class ProdutoAlteracaoRequestDtoValidator : AbstractValidator<ProdutoAlteracaoRequestDto>
    {
        public ProdutoAlteracaoRequestDtoValidator()
        {
            RuleFor(x => x.IdProduto)
               .GreaterThan(0)
               .WithMessage("O identificador do produto é obrigatório.");

            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .MinimumLength(2).WithMessage("O nome deve ter no mínimo 2 caracteres.")
                .MaximumLength(50).WithMessage("O nome deve ter no máximo 50 caracteres.");

            RuleFor(x => x.Descricao)
                .NotEmpty().WithMessage("A descrição é obrigatória.")
                .MinimumLength(5).WithMessage("A descrição deve ter no mínimo 5 caracteres.")
                .MaximumLength(250).WithMessage("A descrição deve ter no máximo 250 caracteres.");

            RuleFor(x => x.Preco)
                .GreaterThan(0).WithMessage("O preço deve ser maior que zero.");

            RuleFor(x => x.Tipo)
                .NotEmpty().WithMessage("O tipo é obrigatório.")
                .MaximumLength(30).WithMessage("O tipo deve ter no máximo 30 caracteres.");

            RuleFor(x => x.Ativo)
                .NotNull().WithMessage("O campo Ativo é obrigatório.");
        }
    }
}
