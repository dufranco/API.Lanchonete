using API.Lanchonete.Domain.DTO.Request;
using FluentValidation;

namespace API.Lanchonete.Domain.Validators
{
    public class ControleAcessoAlteracaoRequestDtoValidator : AbstractValidator<ControleAcessoAlteracaoRequestDto>
    {
        public ControleAcessoAlteracaoRequestDtoValidator()
        {
            RuleFor(x => x.IdControle)
                .GreaterThan(0)
                .WithMessage("O identificador do controle de acesso deve ser maior que zero.");

            RuleFor(x => x.IdPerfil)
                .GreaterThan(0)
                .WithMessage("O identificador do perfil deve ser maior que zero.");

            RuleFor(x => x.NomeTela)
                .NotEmpty()
                .WithMessage("O nome da tela é obrigatório.")
                .MaximumLength(100)
                .WithMessage("O nome da tela deve ter no máximo 100 caracteres.");
        }
    }
}
