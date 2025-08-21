using API.Lanchonete.Domain.DTO.Request;
using FluentValidation;

namespace API.Lanchonete.Domain.Validators
{
    public class PedidoAlteracaoRequestDtoValidator : AbstractValidator<PedidoAlteracaoRequestDto>
    {
        private static readonly string[] statusPermitidos = ["pendente", "em_preparo", "pronto", "entregue"];

        public PedidoAlteracaoRequestDtoValidator()
        {
            RuleFor(x => x.IdPedido)
                .GreaterThan(0)
                .WithMessage("O identificador do pedido deve ser maior que zero.");

            RuleFor(x => x.IdUsuario)
                .GreaterThan(0)
                .WithMessage("O id do usuário deve ser maior que zero.");

            RuleFor(x => x.Status)
                .NotEmpty()
                .WithMessage("O status não pode ser vazio.")
                .MaximumLength(50)
                .WithMessage("O status deve ter no máximo 50 caracteres.");

            RuleFor(x => x.ItensPedido)
                .NotNull()
                .WithMessage("A lista de itens do pedido não pode ser nula.")
                .Must(itens => itens != null && itens.Count != 0)
                .WithMessage("O pedido deve conter pelo menos um item.")
                .Must(itens => itens != null && itens.All(i => i != null))
                .WithMessage("Todos os itens do pedido devem ser válidos (não nulos).");

            RuleForEach(x => x.ItensPedido)
                .ChildRules(item =>
                {
                    item.RuleFor(i => i.IdItem)
                        .GreaterThan(0)
                        .WithMessage("O identificador do item deve ser maior que zero.");

                    item.RuleFor(i => i.IdProduto)
                        .GreaterThan(0)
                        .WithMessage("O identificador do produto deve ser maior que zero.");

                    item.RuleFor(i => i.Quantidade)
                        .GreaterThan(0)
                        .WithMessage("A quantidade do item deve ser maior que zero.");

                    item.RuleFor(i => i.Status)
                        .NotEmpty()
                        .WithMessage("O status do item não pode ser vazio.")
                        .MaximumLength(20)
                        .WithMessage("O status do item deve ter no máximo 20 caracteres.")
                        .Must(status => statusPermitidos.Contains(status))
                        .WithMessage("O status do item deve ser um dos seguintes valores: 'pendente', 'em_preparo', 'pronto', 'entregue'.");
                });
        }
    }
}
