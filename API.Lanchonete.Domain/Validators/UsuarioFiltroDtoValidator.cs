using API.Lanchonete.Domain.DTO.Request.Filtro;
using FluentValidation;

namespace API.Lanchonete.Domain.Validators
{
    public class UsuarioFiltroDtoValidator : AbstractValidator<UsuarioFiltroDto>
    {
        public UsuarioFiltroDtoValidator()
        {
            RuleFor(x => x.Nome)
                .MaximumLength(50)
                .WithMessage("O nome deve ter no máximo 50 caracteres.");

            RuleFor(x => x.Email)
                .MaximumLength(100)
                .WithMessage("O e-mail deve ter no máximo 100 caracteres.");

            RuleFor(x => x.DescricaoPerfil)
                .MaximumLength(100)
                .WithMessage("O perfil deve ter no máximo 100 caracteres.");

            RuleFor(x => x.OrdenarPor)
                .InclusiveBetween(0, 3)
                .When(x => x.OrdenarPor.HasValue)
                .WithMessage("A ordenação deve estar entre 0 e 3.");

            RuleFor(x => x.Pagina)
                .GreaterThan(0)
                .WithMessage("A página deve ser maior que zero.");

            RuleFor(x => x.TamanhoPagina)
                .InclusiveBetween(1, 100)
                .WithMessage("O tamanho da página deve ser maior que zero.");
        }
    }
}
