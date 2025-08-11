using API.Lanchonete.Domain.DTO.Request;
using FluentValidation;

namespace API.Lanchonete.Domain.Validators
{
    public class UsuarioCadastroRequestDtoValidator : AbstractValidator<UsuarioCadastroRequestDto>
    {
        public UsuarioCadastroRequestDtoValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .MinimumLength(2).WithMessage("O nome deve ter no mínimo 2 caracteres.")
                .MaximumLength(50).WithMessage("O nome deve ter no máximo 50 caracteres.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("O e-mail é obrigatório.")
                .MaximumLength(100).WithMessage("O e-mail deve ter no máximo 100 caracteres.")
                .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("O e-mail informado não é válido.");

            RuleFor(x => x.IdPerfil)
                .GreaterThan(0)
                .WithMessage("O perfil é obrigatório.");

            RuleFor(x => x.Senha)
                .NotEmpty().WithMessage("A senha é obrigatória.")
                .MinimumLength(8).WithMessage("A senha deve ter no mínimo 8 caracteres.")
                .MaximumLength(50).WithMessage("A senha deve ter no máximo 50 caracteres.");
        }
    }
}
