using API.Lanchonete.Domain.DTO;
using FluentValidation;

namespace API.Lanchonete.Domain.Validation
{
    public class PerfilDtoValidator : AbstractValidator<PerfilDto>
    {
        public PerfilDtoValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome � obrigat�rio.")
                .MinimumLength(2).WithMessage("O nome deve ter no m�nimo 2 caracteres.")
                .MaximumLength(50).WithMessage("O nome deve ter no m�ximo 50 caracteres.");

            RuleFor(x => x.Descricao)
                .NotEmpty().WithMessage("A descri��o � obrigat�ria.")
                .MaximumLength(250).WithMessage("A descri��o deve ter no m�ximo 250 caracteres.");
        }
    }
}
