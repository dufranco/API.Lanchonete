using API.Lanchonete.Domain.DTO.Request;
using FluentValidation;

namespace API.Lanchonete.Domain.Validators
{
    public class PedidoCadastroRequestDtoValidator : AbstractValidator<PedidoCadastroRequestDto>
    {
        public PedidoCadastroRequestDtoValidator()
        {
            RuleFor(x => x.IdUsuario)
                .GreaterThan(0)
                .WithMessage("O IdUsuario deve ser maior que zero.");

            RuleFor(x => x.ItensPedido)
                .NotNull()
                .WithMessage("A lista de itens do pedido não pode ser nula.")
                .Must(itens => itens != null && itens.Count != 0)
                .WithMessage("O pedido deve conter pelo menos um item.")
                .Must(itens => itens != null && itens.All(i => i != null))
                .WithMessage("Todos os itens do pedido devem ser válidos (não nulos).")
                .Must(itens => itens != null && itens.Select(i => i.IdProduto).Distinct().Count() == itens.Count)
                .WithMessage("Não é permitido itens duplicados no pedido (produto repetido).");

            RuleForEach(x => x.ItensPedido)
                .ChildRules(item =>
                {
                    item.RuleFor(i => i.IdProduto)
                        .GreaterThan(0)
                        .WithMessage("O identificador de cada item deve ser maior que zero.");

                    item.RuleFor(i => i.Quantidade)
                        .GreaterThan(0)
                        .WithMessage("A quantidade de cada item deve ser maior que zero.");
                });
        }
    }
}
