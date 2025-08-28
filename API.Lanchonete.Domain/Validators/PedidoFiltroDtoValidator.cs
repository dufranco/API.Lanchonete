using API.Lanchonete.Domain.DTO.Request.Filtro;
using FluentValidation;

namespace API.Lanchonete.Domain.Validators
{
    public class PedidoFiltroDtoValidator : AbstractValidator<PedidoFiltroDto>
    {
        private static readonly List<string> statusPermitidos = new List<string> { "pendente", "em_preparo", "pronto", "entregue" };

        public PedidoFiltroDtoValidator()
        {

            RuleFor(x => x.Pagina)
                .GreaterThan(0).WithMessage("A página deve ser maior que zero.");

            RuleFor(x => x.TamanhoPagina)
                .GreaterThan(0).WithMessage("O tamanho da página deve ser maior que zero.");

            RuleFor(x => x.DataCadastroFim)
                .GreaterThanOrEqualTo(x => x.DataCadastroIni)
                .When(x => x.DataCadastroIni.HasValue && x.DataCadastroFim.HasValue)
                .WithMessage("A data final deve ser maior ou igual à data inicial.");

            RuleFor(x => x.Status)
                .MaximumLength(50).WithMessage("O status deve ter no máximo 50 caracteres.")
                .Must(status => string.IsNullOrWhiteSpace(status) || statusPermitidos.Contains(status.Trim().ToLower()))
                .WithMessage("O status deve ser 'pendente', 'em_preparo', 'pronto' ou 'entregue'.")
                .When(x => !string.IsNullOrWhiteSpace(x.Status));

            RuleFor(x => x.IdPedido)
                .GreaterThan(0).When(x => x.IdPedido.HasValue)
                .WithMessage("O Id do pedido deve ser maior que zero.");

            RuleFor(x => x.IdProduto)
                .GreaterThan(0).When(x => x.IdProduto.HasValue)
                .WithMessage("O Id do produto deve ser maior que zero.");

            RuleFor(x => x.OrdenarPor)
                .InclusiveBetween(0, 3).When(x => x.OrdenarPor.HasValue)
                .WithMessage("A ordenação deve estar entre 0 e 3.");
        }
    }
}
