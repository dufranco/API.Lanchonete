using API.Lanchonete.Domain.DTO.Request;
using FluentValidation;

namespace API.Lanchonete.Domain.Validators
{
    public class ControleAcessoCadastroRequestDtoValidator : AbstractValidator<ControleAcessoCadastroRequestDto>
    {
        public ControleAcessoCadastroRequestDtoValidator()
        {
            RuleFor(x => x.IdPerfil)
                .GreaterThan(0)
                .WithMessage("O identificador do perfil deve ser maior que zero.");

            RuleFor(x => x.NomeTela)
                .NotEmpty()
                .WithMessage("O nome da tela é obrigatório.")
                .MaximumLength(100)
                .WithMessage("O nome da deve ter no máximo 100 caracteres.");
        }
    }
}
