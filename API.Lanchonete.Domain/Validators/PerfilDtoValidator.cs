using API.Lanchonete.Domain.DTO;
using FluentValidation;

namespace API.Lanchonete.Domain.Validation
{
    public class PerfilDtoValidator : AbstractValidator<PerfilDto>
    {
        public PerfilDtoValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .MinimumLength(2).WithMessage("O nome deve ter no mínimo 2 caracteres.")
                .MaximumLength(50).WithMessage("O nome deve ter no máximo 50 caracteres.");

            RuleFor(x => x.Descricao)
                .NotEmpty().WithMessage("A descrição é obrigatória.")
                .MaximumLength(250).WithMessage("A descrição deve ter no máximo 250 caracteres.");
        }
    }
}
